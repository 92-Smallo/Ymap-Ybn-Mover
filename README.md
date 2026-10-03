# Ymap Ybn Mover

Windows tool for GTA V / FiveM map placement and CodeWalker resaves.

Download the [latest release](https://github.com/92-Smallo/Ymap-Ybn-Mover/releases/latest), extract **all** files into a folder, and run the executable.

## Moving and rotating maps

1. Add all YMAP and YBN files belonging to one map, or add its folder recursively.
2. Enter the move offset X, Y, Z.
3. Enter **Z (degrees)** in the Rotation group. Positive angles rotate counterclockwise viewed from above: +X turns toward +Y. Rotation uses a vertical axis through the map's centre and preserves height.
4. The centre is calculated automatically from the combined entity bounds of all loaded YMAPs. Without usable YMAP bounds, it uses the placed collision shapes. The same centre, angle and offset are applied to every processed YMAP/YBN.
5. Choose **Process All** or **Process Selected**.

The transformation is `newPosition = centre + Rz(angle) * (position - centre) + offset`: rotate around the map centre first, then translate. For example, with centre `(100, 200)`, rotating `(102, 200, 3)` by +90 degrees gives `(100, 202, 3)` before the move offset is added. With zero offset, the centre stays in place. There are no pivot inputs.

**Process Selected** calculates the centre from all loaded map files, so selecting only collision files still uses the loaded YMAP context. Process the whole location together; loading unrelated maps into the same list gives a combined centre. Centre calculation finishes before any file is replaced, and can be cancelled. A zero-degree move/resave does not need a centre calculation.

Collision meshes store their transformed vertices, geometry centres and shrunk vertices in the parent frame, with an identity child matrix. This keeps them visible and selectable in CodeWalker: its mesh renderer reads a 90° matrix as zero X/Y scale, and its polygon queries otherwise apply placement twice. The tool rebuilds mesh lookup data and preserves polygon types, materials and collision filters. Native primitive bounds retain their placement matrices; nested composite frames are preserved. Existing non-uniform scale/shear can be baked for triangle-only meshes. Meshes combining non-uniform scale/shear with primitive polygons are refused because baking would change their shapes.

Vertex packing covers every raw and shrunk vertex, including meshes whose geometry centre differs from the bounding-box midpoint. Outer collision bounds are recalculated from the children, including during zero-offset resaves. A standalone collision root is wrapped in a composite when placement changes. Quarter turns use exact rotation matrices, with the usual precision limits when transformed vertices are packed back into the file.

YMAP transformations include entity and MLO instance placement, car-generator headings, distant light positions, LOD light directions, grass positions/normals, timecycle bounds and occlusion data. Streaming/entity bounds are transformed using all eight corners. Timecycle volumes remain axis-aligned, so arbitrary rotation produces an enclosing box. Packed grass coordinates and occluder coordinates have the precision limits of the game format.

## Resaving models

Supported inputs: `.ymap`, `.ybn`, `.ydr`, `.ydd`, `.yft` (extensions are case-insensitive).

YDR, YDD and YFT files are resaved through CodeWalker; their local model coordinates are preserved. Set the move offset and rotation to zero to resave YMAP and YBN files too. CodeWalker can rebuild polygon edge references and other derived resource data when saving; this is not a guarantee of repairing every model problem.

Zero-offset, zero-degree YBN resaves also bake existing mesh placement matrices without rotating the location again. This can repair files whose collision disappeared from CodeWalker's viewer after conversion by an earlier preview.

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

To check local map fixtures without modifying their originals:

```powershell
dotnet run --project tests/RegressionTests -c Release -- 'E:\testMap\files' artifacts/collision-audit
```

This optional audit calculates one shared map centre, then saves separate collision outputs for resaving, +37°, +90°, -90° and +90° with an offset. It compares all mesh vertices, polygon materials, collision filters, enclosing bounds, renderer scale and sampled contacts through the unmodified loaded CodeWalker hierarchy. It reports nearest-face changes separately from missing contacts, since millimetre packing differences can change which overlapping/grazing face is closest. It also checks YMAP entity positions/orientations at +90°. An optional third directory argument compares earlier outputs rotated +90° about world origin. Game assets are not included in the repository.

To make a portable framework-dependent build:

```powershell
dotnet publish "Ymap Ybn Mover.csproj" -c Release -o artifacts/release
```

Distribute the entire output folder. Users need the .NET 9 Desktop Runtime.

## Feedback and acknowledgements

Report bugs and feature requests through [GitHub Issues](https://github.com/92-Smallo/Ymap-Ybn-Mover/issues).

Game format access and resource rebuilding use [CodeWalker](https://github.com/dexyfex/CodeWalker). Math uses SharpDX.Mathematics.
