using CodeWalker.GameFiles;

namespace Ymap_Ybn_Mover;

public static class FileProcessor
{
    public static async Task ProcessAsync(string filename, MapTransform transform, bool createBackup, CancellationToken cancellationToken)
    {
        var oldData = await File.ReadAllBytesAsync(filename, cancellationToken);
        // Parsing and resource serialization are CPU work and must run off the UI thread.
        var newData = await Task.Run(() => ConvertFile(filename, oldData, transform), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (newData.Length == 0) throw new InvalidDataException("CodeWalker produced an empty file.");
        await ReplaceFileAsync(filename, newData, createBackup, cancellationToken);
    }

    public static byte[] ConvertFile(string filename, byte[] data, MapTransform transform)
    {
        switch (Path.GetExtension(filename).ToLowerInvariant())
        {
            case ".ymap":
                var ymap = new YmapFile();
                ymap.Load(data);
                ymap.Name = Path.GetFileName(filename);
                if (ymap.Meta == null) throw new InvalidDataException("Only resource-format YMAP files can be safely resaved.");
                GameFileTransformer.TransformYmap(ymap, transform);
                var result = ymap.Save();
                if (ymap.SaveWarnings?.Count > 0)
                    throw new NotSupportedException("CodeWalker could not preserve all YMAP data: " + string.Join("; ", ymap.SaveWarnings));
                return result;
            case ".ybn":
                var ybn = new YbnFile();
                ybn.Load(data);
                GameFileTransformer.TransformYbn(ybn, transform);
                return ybn.Save();
            case ".ydr":
                var ydr = new YdrFile();
                RpfFile.LoadResourceFile(ydr, data, 165);
                if (ydr.Drawable == null) throw new InvalidDataException("The YDR contains no drawable.");
                return ydr.Save();
            case ".ydd":
                var ydd = new YddFile();
                RpfFile.LoadResourceFile(ydd, data, 165);
                if (ydd.DrawableDict == null) throw new InvalidDataException("The YDD contains no drawable dictionary.");
                return ydd.Save();
            case ".yft":
                var yft = new YftFile();
                RpfFile.LoadResourceFile(yft, data, 162);
                if (yft.Fragment == null) throw new InvalidDataException("The YFT contains no fragment.");
                return yft.Save();
            default:
                throw new NotSupportedException("Unsupported file extension.");
        }
    }

    public static async Task ReplaceFileAsync(string filename, byte[] data, bool createBackup, CancellationToken cancellationToken)
    {
        filename = Path.GetFullPath(filename);
        var temporary = filename + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            // A sibling temporary file makes the replacement atomic on the same volume.
            await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                81920, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await stream.WriteAsync(data, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            cancellationToken.ThrowIfCancellationRequested();
            var backup = createBackup ? filename + ".bak" : null;
            if (backup != null && File.Exists(backup)) backup = filename + "." + Guid.NewGuid().ToString("N") + ".bak";
            File.Replace(temporary, filename, backup);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }
}
