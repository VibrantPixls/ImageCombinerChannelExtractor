namespace ImageCombinerChannelExtractor.Components.Structs
{
    public readonly record struct CombinedImageTargetStruct
    {
        public int TargetOutputWidth { get; }
        public int TargetOutputHeight { get; }
        public double PreviewImageWidth { get; }
        public double PreviewImageHeight { get; }

        public CombinedImageTargetStruct((int Width, int Height) targetOutput, (double Width, double Height) previewImage)
        {
            TargetOutputWidth = targetOutput.Width;
            TargetOutputHeight = targetOutput.Height;
            PreviewImageWidth = previewImage.Width;
            PreviewImageHeight = previewImage.Height;
        }
    }
}
