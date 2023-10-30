# Text Bob

A simple text editor.

Uses: https://github.com/AvaloniaUI/AvaloniaEdit

## TODO

- Set app version to be used in window title and elsewhere.
- Add context menu with text transformation and other operations.
- Add TextMate themes.
- Add theme selection to config.
- Add font size etc. to config.
- Add drag and drop support (a file can be dropped on the window).
- Add app info to macOS About window and menu.
- Create build scripts for macOS and Windows.
- Add support for multiple languages (English, Czech, ...).

## Keyboard shortcuts

- Copy = CTRL/CMD + C
- Cut = CTRL/CMD + X
- Paste = CTRL/CMD + V
- SelectAll = CTRL/CMD + A
- Undo = CTRL/CMD + Z
- Redo = CTRL/CMD + Y
- Find = CTRL/CMD + F
- Replace = CTRL + H | CMD + ALT + F

- Delete = Delete
- DeleteNextWord = CTRL + Delete
- DeleteLine = CTRL/CMD + D
- Backspace = Backspace
- DeletePreviousWord = CTRL + Backspace
- EnterParagraphBreak = Enter
- EnterLineBreak = SHIFT + Enter
- TabForward = Tab
- TabBackward = SHIFT + Tab
- MoveLeftByCharacter = Left
- SelectLeftByCharacter = SHIFT + Left
- BoxSelectLeftByCharacter = ALT + SHIFT + Left
- MoveRightByCharacter = Right
- SelectRightByCharacter = SHIFT + Right
- BoxSelectRightByCharacter = ALT + SHIFT + Right
- MoveLeftByWord = CTRL + Left
- SelectLeftByWord = SHIFT + CTRL + Left
- MoveRightByWord = CTRL + Right
- SelectRightByWord = SHIFT + CTRL + Right
- MoveUpByLine = Up
- SelectUpByLine = SHIFT + Up
- MoveDownByLine = Down
- SelectDownByLine = SHIFT + Down
- MoveDownByPage = Page Down
- SelectDownByPage = SHIFT + Page Down
- MoveUpByPage = Page Up
- SelectUpByPage = SHIFT + Page Up
- MoveToLineStart = Home
- SelectToLineStart = SHIFT + Home
- MoveToLineEnd = End
- SelectToLineEnd = SHIFT + End
- MoveToDocumentStart = CTRL + Home
- SelectToDocumentStart = SHIFT + CTRL + Home
- MoveToDocumentEnd = CTRL + End
- SelectToDocumentEnd = SHIFT + CTRL + End
- IndentSelection = CTRL/CMD + I

## Building for macOS

To create app package for macOS, run following command in the folder with the `build-macos.sh` build script:

```
bash ./build-macos.sh
```

Resulting package structure:

```
TextBob.app
    Contents\
        _CodeSignature\
            CodeResources
        MacOS\
            TextBob
            TextBob.dll
            Avalonia.dll
            ...
        Resources\
            TerxtBob.icns (icon file)
        Info.plist
        embedded.provisionprofile
```

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

App uses .NET 8.0.

```
# For Windows:
dotnet publish -c Release --runtime win-x64 --force --self-contained true -p:PublishSingleFile=true

# For macOS (arm64):
dotnet publish -c Release --runtime osx-arm64 --force --self-contained true -p:PublishSingleFile=true
```

## Links

- https://avaloniaui.net/
- https://avaloniaui.github.io/icons.html
- https://docs.avaloniaui.net/docs/next/guides/platforms/macos-development
- https://docs.avaloniaui.net/docs/next/deployment/macOS
- https://learn.microsoft.com/en-us/windows/apps/design/style/iconography/app-icon-construction
