using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Evacuation_Master_3000.ImageScan
{
    internal static class ImageScanningTools
    {
        public static Bitmap ResizeIfNecessary(Bitmap theImage, double maxWidth, double maxHeight)
        {
            double imageWidth = theImage.Width;
            double imageHeight = theImage.Height;
            if (imageWidth > maxWidth || theImage.Height > maxHeight)
            {

                double scalingFactorWidth = maxWidth / imageWidth;
                double scalingFactorHeight = maxHeight / imageHeight;
                double scalingFactor = scalingFactorWidth < scalingFactorHeight ? scalingFactorWidth : scalingFactorHeight;
                return ResizeImage(theImage, (int)(imageWidth * scalingFactor), (int)(imageHeight * scalingFactor));
            }
            // Simply returns the same image if resizing wasn't necessary.
            return theImage;
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        public static double[,] ConvertImageToGrayscale(Bitmap image)
        {
            double[,] pixels = new double[image.Height, image.Width];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    pixels[y, x] = ConvertPixelToGrayscale(image.GetPixel(x, y));
                }
            }
            return pixels;
        }

        private static double ConvertPixelToGrayscale(System.Drawing.Color pixel)
        {
            return (double)(pixel.R + pixel.G + pixel.B) / 3;
        }

        public static double[,] ApplySobelFilter(double[,] pixels, int width, int height)
        {
            double[,] localPixelSet = new double[height, width];
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {

                    // a b c
                    // d X e
                    // f g h
                    // 
                    // X is the pixel we are currently looking at.

                    double a = pixels[y - 1, x - 1],
                        b = pixels[y - 1, x],
                        c = pixels[y - 1, x + 1],
                        d = pixels[y, x - 1],
                        e = pixels[y, x + 1],
                        f = pixels[y + 1, x - 1],
                        g = pixels[y + 1, x],
                        h = pixels[y + 1, x + 1];

                    double edgeVertical = (f + 2 * g + h) - (a + 2 * b + c);

                    double edgeHorizontal = (a + 2 * d + f) - (c + 2 * e + h);

                    localPixelSet[y, x] = Math.Sqrt(Math.Pow(edgeVertical, 2) + Math.Pow(edgeHorizontal, 2));
                }
            }
            return localPixelSet;
        }

    }
}