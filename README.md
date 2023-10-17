# Text Bob

A simple text editor.

Uses: https://github.com/AvaloniaUI/AvaloniaEdit

## TODO

- Add a configuration in a JSON file.
- Tab: increase text indentation of selected lines (or just insert the TAB character, if the selection is none or does not cover multiple lines)
- SHIFT + Tab: decrease text indentation of selected lines (or nothing, if the selection is none or does not cover multiple lines)
- Alt: during selection switch to column selection and allow editing at multiple places at the same time. Allow cursor to go beyond the line end.
- Add support for multiple languages (English, Czech, ...).

## How to create macOS icons file

You can achieve this using the `sips` command.

First, store your icon as follows:

- In an image file of size 1024 x 1024 pixels
- In png format
- In a file named Icon1024.png

Then execute the following commands

```
mkdir MyIcon.iconset

sips -z 16 16     Icon1024.png --out TextBob.iconset/icon_16x16.png
sips -z 32 32     Icon1024.png --out TextBob.iconset/icon_16x16@2x.png
sips -z 32 32     Icon1024.png --out TextBob.iconset/icon_32x32.png
sips -z 64 64     Icon1024.png --out TextBob.iconset/icon_32x32@2x.png
sips -z 128 128   Icon1024.png --out TextBob.iconset/icon_128x128.png
sips -z 256 256   Icon1024.png --out TextBob.iconset/icon_128x128@2x.png
sips -z 256 256   Icon1024.png --out TextBob.iconset/icon_256x256.png
sips -z 512 512   Icon1024.png --out TextBob.iconset/icon_256x256@2x.png
sips -z 512 512   Icon1024.png --out TextBob.iconset/icon_512x512.png

cp Icon1024.png TextBob.iconset/icon_512x512@2x.png

iconutil -c icns TextBob.iconset
```

The result will be a file named `TextBob.icns` that you can use to add to your .app directory.

Source: https://stackoverflow.com/questions/12306223/how-to-manually-create-icns-files-using-iconutil


## How to publish

MacOS: https://docs.avaloniaui.net/docs/next/deployment/macOS

```
# For Windows:
dotnet publish -c Release --runtime win-x64 --force --self-contained true -p:PublishSingleFile=true

# For macOS (arm64):
dotnet publish -c Release --runtime osx-arm64 --force --self-contained true -p:PublishSingleFile=true
```

## Links

- https://avaloniaui.net/
- https://docs.avaloniaui.net/docs/next/guides/platforms/macos-development
- https://docs.avaloniaui.net/docs/next/deployment/macOS
- https://learn.microsoft.com/en-us/windows/apps/design/style/iconography/app-icon-construction
