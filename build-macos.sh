#!/bin/bash
APP=$PWD
APP_SRC="$APP/src"
APP_NAME="$APP/publish/macOS/TextBob.app"

# Publish the app.
cd "$APP_SRC" || exit
dotnet publish -c Release --runtime osx-arm64 --force --self-contained true -p:PublishSingleFile=true
cd "$APP" || exit

# PUBLISH_OUTPUT_DIRECTORY should point to the output directory of your dotnet publish command.
# One example is /path/to/your/csproj/bin/Release/netcoreapp3.1/osx-x64/publish/.
# If you want to change output directories, add `--output /my/directory/path` to your `dotnet publish` command.
PUBLISH_OUTPUT_DIRECTORY="$PWD/src/TextBob/bin/Release/net7.0/osx-arm64/publish/."

INFO_PLIST="$APP_SRC/TextBob/Assets/macOS/Info.plist"
ICON_FILE="$APP_SRC/TextBob/Assets/macOS/TextBob.icns"

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir -p "$APP_NAME"

mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/TextBob.icns"
cp -a "$PUBLISH_OUTPUT_DIRECTORY" "$APP_NAME/Contents/MacOS"
