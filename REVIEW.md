# Project review and rotation implementation

Reviewed 3 October 2026. This review uses the installed CodeWalker.Core build and its matching local source, with the upstream [Bounds implementation](https://github.com/dexyfex/CodeWalker/blob/master/CodeWalker.Core/GameFiles/Resources/Bounds.cs) as an additional reference.

## Findings addressed

| Finding | Impact and change |
| --- | --- |
| YMAP entities were translated twice | Removed the duplicate loop; translation is applied once through one transform service. |
| YBN movement edited local child bounds and centers as world coordinates | Apply placement through child matrices. Keep vertices, geometry centers, primitive dimensions and nested relative transforms in local coordinates. |
| Unfinished rotation mixed world placement with shape geometry | Added Z rotation around one automatic map centre, explicit rotate-then-move order, entity orientations and car headings, and eight-corner bounding-box transforms. |
| World-origin rotation swung the map away from its location | Calculate the centre of all loaded YMAP entity extents before writing any file. Fall back to placed collision leaf bounds, ignoring stale composite root boxes. Share that centre across YMAP/YBN conversions, including Process Selected; zero-degree resaves skip the preflight. |
| Fixed footer coordinates and mixed fonts caused crowding at larger display scales | Group move offsets and Z rotation in measured table layouts, separate the action row, remove pivot inputs, and allow window resizing. |
| CodeWalker geometry BVH rebuild used editor-transformed polygon positions | Added `LocalSpaceBoundBvh`: preserve resource fields and build/serialize the mesh BVH with an identity editor transform, then restore placement. Parent matrices remain serialized normally. |
| CodeWalker recalculated vertex quantum from box width and silently clamped off-center raw vertices | The supplied fixtures reproduced displacement of up to 3.6675 m with the earlier converter. Repack the actual vertex resource blocks with a quantum covering all raw/shrunk vertices and at least the original range. Apply this to Geometry and GeometryBVH, preserving geometry centers and polygon/material data. |
| Some supplied collision outer boxes did not enclose their meshes | Recompute composite boxes and enclosing spheres from child placement throughout the hierarchy, including zero-offset resaves. |
| Quaternion-derived quarter-turn matrices introduced small coordinate drift | Use exact 0/±1 coefficients for quarter turns. This removes drift that changed the nearest hit for some tightly overlapping triangles in real fixtures. |
| Standalone YBN roots had no persisted placement matrix | Wrap them in a composite and give the child a stored placement and collision-query filter. |
| Loaded standalone YMAP LOD lights were cleared by CodeWalker.Save | Populate the editor light objects from all raw light arrays before saving; preserve metadata and rotate directions. Parent distant-light positions are not needed for serialization. |
| Grass rotation could change batch bounds without moving individual instances correctly | Decode packed positions against the old bounds, rotate, and repack against the new bounds. Rotate terrain normal X/Y components and preserve other instance attributes. |
| Timecycle and occlusion data were left behind | Transform timecycle bounds, occluder placement and occlusion triangle coordinates alongside the entities. Refuse out-of-range packed occluder positions. |
| Direct writes could truncate originals | Write a complete sibling temporary file and atomically replace the original. Backups default on and use distinct names on repeated processing. |
| Unbounded tasks, shared counters and cross-thread UI updates | Process one conversion at a time off the UI thread, update controls after awaited work, use a Windows Forms timer, and retain cross-thread checks. This also bounds peak memory and avoids concurrent CodeWalker cache access. |
| Cancellation only checked before starting tasks | Pass cancellation through reads/writes, check again before replacement, skip pending files, reset state for each batch, and wait before closing. |
| Duplicate files could enter the same batch; uppercase extensions were ignored during processing | Normalize full paths, deduplicate case-insensitively including within the incoming batch, and dispatch extensions case-insensitively. |
| Automatic update checks blocked startup and treated every unequal version as newer | Use asynchronous requests with a timeout, compare parsed versions, keep automatic failures quiet, and open the release page without claiming a download happened. |
| Hard-coded developer DLL path and unused packages | Use `lib/CodeWalker.Core.dll` or an MSBuild path override, document setup, remove unused Octokit/Newtonsoft.Json, and use System.Text.Json. Target ordinary Windows Forms without the unused Windows SDK/WinRT assemblies. |
| Coordinate parsing depended on the user's locale; map center averaged file centers | Parse/format comma-separated coordinates invariantly. Calculate the center of the selected YMAPs' combined bounds. |
| Unsupported sections could silently disappear on resave | Refuse container LODs, prop instance batches and incomplete YBN cloth bounds; stop on CodeWalker YMAP save warnings before replacing files. |

## Validation

The regression runner constructs synthetic resource files, serializes them through the installed CodeWalker library, transforms them through the production conversion path, then reloads and checks the output. It covers ordinary and MLO entities, car headings, both light types, grass, timecycle/occlusion data, standalone collision primitives, Geometry and GeometryBVH with triangle/sphere/capsule/cylinder/box polygons, composite BVHs with null children, nested placement, existing scaled mesh placement, preserved collision filters, YDR/YDD/YFT resaves, cancellation, corrupt-input preservation, and repeated backups.

The collision query test checks the persisted mesh in its local frame and checks the saved parent placement separately. CodeWalker's editor `GetVertexPos` also applies the editor transform while composite ray queries already inverse-transform the ray; calling its ordinary loaded-composite query directly would mix these frames. This test validates serialized geometry and BVH data without relying on that editor behavior.

All 32 synthetic regression checks pass, including centre invariance, shared YMAP/YBN placement, nested collision-only centre calculation and cancelled preflight. The optional external-asset audit also tested nine user-supplied collision files containing 184,818 vertices, plus three YMAPs containing 822 entities. Resaves, +37°, +90°, -90° and +90° with offset (20, -30, 7) around the shared map centre retained polygon/material signatures and filters; repaired bounds enclosed their children. Vertex errors were at most 0.305 mm for +37° and zero at the reported precision for quarter turns. All 10,655 sampled ray comparisons on the original backup agreed after serialization; 10,680 comparisons on the user's subsequently rotated working folder also agreed. Entity positions/orientations agreed with the +90° transform. Input asset bytes remained unchanged. Assets and generated outputs stay local and are not committed.

The original map's combined centre is approximately X=-2202.611, Y=-539.249. A zero-offset rotation keeps this centre fixed rather than swinging the location around world origin. Ray checks compare the same effective local ray on the input and output mesh, with a separate world/local round-trip tolerance: sub-millimetre float drift can change the nearest face on overlapping triangles even when mesh data is identical. These checks establish serializer and lookup invariants, not game-runtime physics validation.

The form is rendered off-screen to inspect control placement. Measured table layouts are checked at the default size, minimum window size and a larger text size for clipping and overlap. This is not an interactive UI or game-runtime test.

Tested dependency: CodeWalker.Core file version 1.0.0.0, SHA-256 `70A1A48E70941824DCD89A2431FEDD2A62B177717ED2BE676BBA8AF69BA55A38`. The dependency is supplied locally, not committed. These results are specific to that build; rerun regressions before upgrading it.

## Remaining work and recommendations

1. **Validate representative maps in FiveM/GTA V before release.** Real fixtures pass local serialization and sampled collision-query checks. Confirm collision contact, raycasts, streaming, lighting, vegetation, MLO entry and car spawning in the game after rotation. Include both positive and negative angles and translated offsets. These checks do not prove the game's physics behavior or CodeWalker's interactive collision selection.
2. **Add legally shareable regression fixtures and pin CodeWalker by revision/build.** The upstream assembly version is not sufficient to identify API or serializer behavior. A reproducible dependency build would also allow meaningful GitHub Actions checks without relying on a developer's local DLL.
3. **Keep legacy and Enhanced resource support explicit.** The existing resave path uses CodeWalker's legacy resource defaults. Enhanced/Gen9 conversion needs separate fixtures and format handling before it can be claimed as supported.
4. **Extend map-family coverage as needed.** YTYP local archetype data, YNV navigation, YND road paths, YLD cloth and script coordinates are outside the supported input formats. An entire location may rely on those files even when its YMAP/YBN placement is correct. CodeWalker's cloth-bound handling is incomplete; the tool refuses YBN cloth bounds.
5. **Consider an output-folder mode and a transformation preview.** Backups and atomic replacement address the immediate overwrite risk. An output tree and a saved transform manifest would make large migration workflows easier to inspect and repeat.
6. **Consider rotation around X/Y only as a separate feature.** Vehicle spawn headings and box occluders encode horizontal rotation, timecycle volumes are axis-aligned, and foliage normals have limited encoding. The implemented rotation deliberately follows the existing map-heading requirement: Z rotation.
7. **Resave repairs remain dependent on CodeWalker.** The model fixtures exercise format dispatch and stable local bounds, rather than proving repairs for every broken shader, skeleton, fragment, cloth or polygon-edge case.
