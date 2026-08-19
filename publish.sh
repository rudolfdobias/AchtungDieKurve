#!/usr/bin/env bash
# Builds self-contained, single-file releases of AchtungDieKurve.
# Output: publish/<rid>/ plus an archive per target in publish/.
#
# Notes:
# - Trimming/NativeAOT must stay OFF: powerups are instantiated via
#   reflection (Game/Drawable/Powerups/Register.cs).
# - Content .xnb files cannot be embedded in the single file; each
#   release is one executable plus a Content/ folder.
# - Native libraries (SDL2, OpenAL) must stay as loose files next to the
#   executable: MonoGame's loader only searches beside the exe, not the
#   single-file self-extract directory.
# - The macOS binary is unsigned and un-notarized. It runs locally, but
#   distributing it to other Macs requires codesign + notarization.
set -euo pipefail
cd "$(dirname "$0")"

PROJECT=AchtungDieKurve/AchtungDieKurve.csproj
RIDS=(win-x64 osx-arm64 linux-x64)

rm -rf publish
for RID in "${RIDS[@]}"; do
    echo "==> Publishing $RID"
    dotnet publish "$PROJECT" -c Release -r "$RID" --self-contained \
        -p:PublishSingleFile=true \
        -o "publish/$RID"

    echo "==> Archiving $RID"
    case "$RID" in
        win-*) (cd publish && zip -qr "AchtungDieKurve-$RID.zip" "$RID") ;;
        *)     tar -czf "publish/AchtungDieKurve-$RID.tar.gz" -C publish "$RID" ;;
    esac
done

echo
echo "Done:"
ls -lh publish/*.zip publish/*.tar.gz
