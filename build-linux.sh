#!/bin/bash
APP=$PWD
APP_SRC="$APP/src"
APP_NAME="$APP/publish/linux/TextBob"

VersionName=1.1.0

# Set the language to English.
export DOTNET_CLI_UI_LANGUAGE=en

# Publish the app.
cd "$APP_SRC" || exit
dotnet publish ./TextBob/TextBob.csproj -c Release --runtime linux-x64 --force --self-contained true -p:PublishSingleFile=true
cd "$APP" || exit

# PUBLISH_OUTPUT_DIRECTORY should point to the output directory of your dotnet publish command.
# One example is /path/to/your/csproj/bin/Release/netcoreapp3.1/osx-x64/publish/.
# If you want to change output directories, add `--output /my/directory/path` to your `dotnet publish` command.
PUBLISH_OUTPUT_DIRECTORY="$PWD/src/TextBob/bin/Release/net8.0/linux-x64/publish/."

ICONS_DIR="$APP_SRC/TextBob/Assets/macOS/TextBob.iconset"
ICON_FILE="$ICONS_DIR/icon_512x512.png"

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir -p "$APP_NAME/DEBIAN"
mkdir -p "$APP_NAME/usr/bin"
mkdir -p "$APP_NAME/usr/lib/TextBob"
mkdir -p "$APP_NAME/usr/share/applications"
mkdir -p "$APP_NAME/usr/share/icons/hicolor/16x16/apps"
mkdir -p "$APP_NAME/usr/share/icons/hicolor/32x32/apps"
mkdir -p "$APP_NAME/usr/share/icons/hicolor/64x64/apps"
mkdir -p "$APP_NAME/usr/share/icons/hicolor/128x128/apps"
mkdir -p "$APP_NAME/usr/share/icons/hicolor/256x256/apps"
mkdir -p "$APP_NAME/usr/share/icons/hicolor/512x512/apps"
mkdir -p "$APP_NAME/usr/share/pixmaps"

cp "$ICON_FILE" "$APP_NAME/usr/share/pixmaps/TextBob.png"
cp "$ICONS_DIR/icon_16x16.png" "$APP_NAME/usr/share/icons/hicolor/16x16/apps/TextBob.png"
cp "$ICONS_DIR/icon_32x32.png" "$APP_NAME/usr/share/icons/hicolor/32x32/apps/TextBob.png"
cp "$ICONS_DIR/icon_64x64.png" "$APP_NAME/usr/share/icons/hicolor/64x64/apps/TextBob.png"
cp "$ICONS_DIR/icon_128x128.png" "$APP_NAME/usr/share/icons/hicolor/128x128/apps/TextBob.png"
cp "$ICONS_DIR/icon_256x256.png" "$APP_NAME/usr/share/icons/hicolor/256x256/apps/TextBob.png"
cp "$ICONS_DIR/icon_512x512.png" "$APP_NAME/usr/share/icons/hicolor/512x512/apps/TextBob.png"

cp "$APP_SRC/TextBob/Assets/linux/control.txt" "$APP_NAME/DEBIAN/control"
cp "$APP_SRC/TextBob/Assets/linux/textbob.sh" "$APP_NAME/usr/bin/textbob"
cp "$APP_SRC/TextBob/Assets/linux/TextBob.desktop" "$APP_NAME/usr/share/applications/TextBob.desktop"

cp "$PUBLISH_OUTPUT_DIRECTORY/../libHarfBuzzSharp.so" "$APP_NAME/usr/lib/TextBob/libHarfBuzzSharp.so"
cp "$PUBLISH_OUTPUT_DIRECTORY/../libSkiaSharp.so" "$APP_NAME/usr/lib/TextBob/libSkiaSharp.so"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/usr/lib/TextBob"

# set executable permissions to starter script
chmod +x "$APP_NAME/usr/bin/textbob"

# for x64 architectures, the suggested suffix is amd64.
dpkg-deb --root-owner-group --build "$APP_NAME" "./publish/linux/textbob_${VersionName}_amd64.deb"

# .NET 8 dependencies; see control.txt 
# dotnet-hostfxr-8.0, libicu74, libc6 (>= 2.38), libgcc-s1 (>= 3.0), liblttng-ust1t64 (>= 2.13.0), libssl3t64 (>= 3.0.0), libstdc++6 (>= 13.1), zlib1g (>= 1:1.1.4)


# https://docs.avaloniaui.net/docs/deployment/debian-ubuntu


# To install
# sudo apt install ./publish/linux/textbob_1.1.0_amd64.deb

# To uninstall / remove
# sudo apt remove textbob