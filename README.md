# Ymap Ybn Mover

Windows tool for GTA V / FiveM map placement and CodeWalker resaves.

Download the [latest release](https://github.com/92-Smallo/Ymap-Ybn-Mover/releases/latest), extract **all** files into a folder, and run the executable.

## Moving and rotating maps

1. Add your YMAP and YBN files, or add a folder recursively.
2. Enter the move offset X, Y, Z.
3. Enter **Z (degrees)** in the Rotation group. Positive angles rotate counterclockwise viewed from above: +X turns toward +Y. Rotation uses the world's Z axis at X=0, Y=0 and preserves height.
4. Apply exactly the same angle and offset to every YMAP/YBN belonging to that location.
5. Choose **Process All** or **Process Selected**.

The transformation is `newPosition = Rz(angle) * position + offset`: rotate around the world origin first, then translate. For example, rotating `(12, 20, 3)` by +90 degrees gives `(-20, 12, 3)` before the move offset is added. There are no pivot inputs.

Collision shapes retain their local geometry. The tool changes composite child transforms and rebuilds their lookup data through CodeWalker, including meshes, primitive bounds, existing transformed children and nested composites. A standalone collision root is wrapped in a composite so its placement can be stored in the game format.

YMAP transformations include entity and MLO instance placement, car-generator headings, distant light positions, LOD light directions, grass positions/normals, timecycle bounds and occlusion data. Streaming/entity bounds are transformed using all eight corners. Timecycle volumes remain axis-aligned, so arbitrary rotation produces an enclosing box. Packed grass coordinates and occluder coordinates have the precision limits of the game format.

## Resaving models

Supported inputs: `.ymap`, `.ybn`, `.ydr`, `.ydd`, `.yft` (extensions are case-insensitive).

YDR, YDD and YFT files are resaved through CodeWalker; their local model coordinates are preserved. Set the move offset and rotation to zero to resave YMAP and YBN files too. CodeWalker can rebuild polygon edge references and other derived resource data when saving; this is not a guarantee of repairing every model problem.

## Backups, errors and stopping

Processing replaces the input files. **Keep backups** is enabled by default. The first previous version is saved as `filename.ext.bak`; subsequent backups receive unique names and never overwrite earlier backups. To restore one, copy it back to the original filename with the original extension.

Each output is fully generated and written to a sibling temporary file before the original is atomically replaced. Conversion/save failures leave the original untouched. The Status column shows progress; hover over an error row for its full message.

**Stop** cancels pending work and prevents the current file being replaced if cancellation arrives before replacement. A synchronous CodeWalker conversion must finish before cancellation takes effect. Files already marked Done remain processed. Closing during a batch requests cancellation and waits for processing to finish.

The tool refuses YMAP container LODs and prop instance batches because the current CodeWalker saver cannot preserve them. YBN cloth bounds are also refused because that serializer is incomplete. Save warnings prevent replacement. Occluder centers outside the format's signed-short coordinate range (approximately +/-8192 m) are refused.

## Building and checking changes

Install the .NET 9 SDK on Windows. Supply `CodeWalker.Core.dll` from your CodeWalker build in `lib/`; it is intentionally excluded from Git. The local dependency must be available for both application builds and regression checks.

```powershell
dotnet build "Ymap Ybn Mover.sln" -c Release
dotnet run --project tests/RegressionTests -c Release
```

Alternatively, supply an existing DLL path to each command:

```powershell
dotnet build "Ymap Ybn Mover.sln" -c Release '-p:CodeWalkerCorePath=C:\CodeWalker\CodeWalker.Core.dll'
dotnet run --project tests/RegressionTests -c Release '-p:CodeWalkerCorePath=C:\CodeWalker\CodeWalker.Core.dll'
```

The regression runner needs no test-framework packages. It returns a nonzero exit code on failure and checks serialized YMAP/YBN transformations, local collision BVHs, nested/scaled placement, collision filters, model-format resaves, backups and cancellation. Run it whenever the CodeWalker dependency changes. [REVIEW.md](REVIEW.md) records the findings and remaining validation work.

To make a portable framework-dependent build:

```powershell
dotnet publish "Ymap Ybn Mover.csproj" -c Release -o artifacts/release
```

Distribute the entire output folder. Users need the .NET 9 Desktop Runtime.

## Feedback and acknowledgements

Report bugs and feature requests through [GitHub Issues](https://github.com/92-Smallo/Ymap-Ybn-Mover/issues).

Game format access and resource rebuilding use [CodeWalker](https://github.com/dexyfex/CodeWalker). Math uses SharpDX.Mathematics.
