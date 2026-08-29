# Image Combiner & Channel Extractor

> [!NOTE]
> This is the **modern** version of the Image Combiner & Channel Extractor.
> Looking for a legacy version? Check out the legacy version [here](https://github.com/VibrantPixls/ImageCombinerChannelExtractorLegacy)!

Image Combiner & Channel Extractor is a Windows WPF (.NET) application designed for game developers and texture artists. It provides a simple way to pack or unpack texture maps for engines like Unity and Unreal Engine.
* Combine up to four images into a single RGBA PNG.
  * Assign each input image to a color channel (Red, Green, Blue, Alpha).
* Extract individual color channels (Red, Green, Blue, Alpha) from an existing image into new standalone grayscale image files.

## Features
#### Multiple supported input image formats
* Import PNG or JPG/JPEG images for each channel.
#### Automatic grayscale conversion
* Automatically converts colored input images to grayscale.
#### Image previews
* Preview images before combining or extracting.
#### Supports automatic image resizing
* Supports input images with different dimensions.
#### Option to change the image resampling
* Choose between Bicubic, Bilinear, or Nearest Neighbor resampling.
#### Drag and drop
* Quickly assign input files by dragging them directly into the interface.
#### PNG output
* Save outputs as .png (with transparency).

<br>

> [!NOTE]
> **Two versions are provide**
> * **Framework-dependent**: Requires **.NET 10.0 Desktop Runtime** to be installed on your system.
> * **Self-contained**: Standalone executable that comes out of the box with everything included.

## Prerequisites
* Windows 10 or Windows 11
* [.NET 10.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (only for framework dependent version)

> [!IMPORTANT]
> If Windows SmartScreen appears, click **More Info** and then **Run Anyway**.
>
> *This warning occurs because the executable is not digitally signed, as code signing certificates require significant recurring expenses on the developers part.*

## Usage
### Combine Channels
1. **Select** up to four grayscale images for the Red, Green, Blue, and Alpha channels.
2. Click **Create Combined PNG**.
3. Choose your output destination and **save** your output image.

![Combining channels](/ReadmeAssets/readme_explain_combine.png)
![Looks of the combine panel](/ReadmeAssets/readme_preview_combiner.png)

### Extract Channels
1. **Select** an existing PNG, JPG, or JPEG file.
2. Click on **Download [image]** of your desired color channel.
3. **Save** your extracted grayscale image.

![Extracting channels](/ReadmeAssets/readme_explain_extractor.png)
![Looks of the extract panel](/ReadmeAssets/readme_preview_extractor.png)
