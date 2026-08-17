# Image Combiner & Channel Extractor

> [!IMPORTANT]
> This version is currently being worked on and is not yet finished.

> [!NOTE]
> This is the **modern** version of the Image Combiner & Channel Extractor.
> Looking for a legacy version? Check out the legacy version [here](https://github.com/VibrantPixls/ImageCombinerChannelExtractorLegacy)!

Image Combiner is a Windows WPF (.NET) application that lets you:
* Combine up to four images into a single RGBA PNG.
* Assign each input to a color channel (Red, Green, Blue, Alpha).
* Extract individual color channels (Red, Green, Blue, Alpha) from an existing image into new files.

Export results as PNG (with transparency).

This tool is designed for game developers and texture artists, who need to pack or unpack texture maps efficiently.

## Features

* Import PNG or JPG images for each channel.
* Automatic grayscale conversion of inputs.
* Fast pixel manipulation.
* Preview images before combining or extracting.
* Save outputs as .png (with transparency).

## Prerequisites
* Windows 10/11
* .NET 10.0

## Usage
### Combine Channels
1. Select up to four grayscale images for R, G, B, and A channels.
2. Click Create Combined PNG.
3. Save your final image.

![Combing channels](/docs/images/previewCombined.png)
![Looks of the combine panel](/docs/images//previewimageProgramLook01.png)

### Extract Channels
1. Load an existing PNG or JPG.
2. Click on Download channel.
3. Save your extracted image.

![Combing channels](/docs/images//previewExtracted.png)
![Looks of the extract panel](/docs/images//previewimageProgramLook02.png)

## Keywords for SEO
image combiner, channel extractor, pack textures, unpack textures, grayscale to RGBA, Unity textures, Unreal Engine textures, alpha channel extractor, combine PNG channels, split color channels, texture map creator


