using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Evacuation_Master_3000.UI.ControlPanelUI;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Remoting.Channels;
using System.Windows.Shapes;

namespace Evacuation_Master_3000.ImageScan
{
    /// <summary>
    /// Interaction logic for CP_ImageScanpicture.xaml
    /// </summary>
    public partial class CP_ImageScanpicture : UserControl
    {

        private static Bitmap _theImage;
        private static double[,] _pixels;
        private int maxWidth = 1000;
        private int maxHeight = 1000;
        private bool applySobelFilter = false;



        public CP_ImageScanpicture(ImageScanWindow parentWindow, string imageFilePath)
        {
            InitializeComponent();
            ParentWindow = parentWindow;
            ImageToGridMethod( imageFilePath, maxWidth, maxHeight, applySobelFilter);
            Console.WriteLine(imageFilePath);
        }

        private ImageScanWindow ParentWindow { get; }
        public void ImageToGridMethod(string imageFilePath, int maxWidth, int maxHeight, bool applySobelFilter)
        {
            if (!File.Exists(imageFilePath))
            {
                throw new FileNotFoundException();
            }
            _theImage = ResizeIfNecessary(new Bitmap(imageFilePath), maxWidth, maxHeight);

            _pixels = ConvertImageToGrayscale(_theImage);

            if (applySobelFilter)
            {
                 ApplySobelFilter(ref _pixels, _theImage.Width, _theImage.Height);
            }
            CreateVisualRepresentation(_pixels, _theImage.Width, _theImage.Height);
            // CreateGridFile(gridFilePath, header, description, contrastTurnOver, _pixels, _theImage.Width, _theImage.Height);
        }

        private void CreateVisualRepresentation(double[,] pixels, int width, int height)
        {
            double threshold = ParentWindow.CpImageScanControls.ContrastSlider.Value;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle()
                    {
                        Height = 10,
                        Width = 10,
                        Fill =
                            pixels[y, x] >= threshold
                                ? new SolidColorBrush(Colors.Yellow)
                                : new SolidColorBrush(Colors.Black)
                    };
                    BuildingBlockContainer.Children.Add(rect);
                }
            }
        }


        public void ContrastChanger(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //_pixels += ParentWindow.CpImageScanControls.ContrastSlider
        }


        private Bitmap ResizeIfNecessary(Bitmap theImage, double maxWidth, double maxHeight)
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
            // Simply returns the same image if no resizing wasn't necessary.
            return theImage;
        }

        private Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
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

        private double[,] ConvertImageToGrayscale(Bitmap image)
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

        private static void ApplySobelFilter(ref double[,] pixels, int width, int height)
        {
            double[,] localPixelSet = new double[height, width];
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double edgeVertical = pixels[y + 1, x - 1] + 2 * pixels[y + 1, x] + pixels[y + 1, x + 1] -
                        (pixels[y - 1, x - 1] + 2 * pixels[y - 1, x] + pixels[y - 1, x + 1]);

                    double edgeHorizontal = pixels[y - 1, x + 1] + 2 * pixels[y, x + 1] + pixels[y + 1, x + 1] -
                        (pixels[y - 1, x - 1] + pixels[y, x - 1] + pixels[y + 1, x - 1]);

                    localPixelSet[y, x] = Math.Sqrt(Math.Pow(edgeVertical, 2) + Math.Pow(edgeHorizontal, 2));
                }
            }
            pixels = localPixelSet;
        }

        /// <summary>
        /// Creates a new .grid file based on the given input image.
        /// </summary>
        /// <param name="filePath">Output filepath</param>
        /// <param name="header">Header / title</param>
        /// <param name="description"></param>
        /// <param name="contrastTurnOver">The RGB value (0 - 255) where it differentiates between a wall or free. 100 seems to do the trick.</param>
        private void CreateGridFile(string filePath, string header, string description, int contrastTurnOver, double[,] pixels, int imageWidth, int imageHeight)
        {
            StringBuilder gridFileText = new StringBuilder();
            gridFileText.Append($"<Settings>{Environment.NewLine}<Width>{imageWidth}</Width>{Environment.NewLine}<Height>{imageHeight}</Height>{Environment.NewLine}<Header>{header}</Header>{Environment.NewLine}<Description>{description}</Description>{Environment.NewLine}</Settings>{Environment.NewLine}<Grid>{Environment.NewLine}");
            for (int x = 1; x < imageWidth; x++)
            {
                gridFileText.Append("<Row>");
                for (int y = 1; y < imageHeight; y++)
                {
                    gridFileText.Append(pixels[y, x] < contrastTurnOver ? "3" : "0");
                }
                gridFileText.Append($"</Row>{Environment.NewLine}");
            }
            gridFileText.Append("</Grid>");
            try
            {
                File.WriteAllText(filePath, gridFileText.ToString());
            }
            catch (Exception e)
            {

                Console.WriteLine(@"Something went wrong with creating the gridfile: " + e);
            }
        }



        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
    }
}
