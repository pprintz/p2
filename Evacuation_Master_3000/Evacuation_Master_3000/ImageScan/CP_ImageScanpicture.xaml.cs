using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Evacuation_Master_3000.ImageScan
{
    /// <summary>
    /// Interaction logic for CP_ImageScanPicture.xaml
    /// </summary>
    public partial class CP_ImageScanPicture
    {

        private static Bitmap _theImage;
        public double[,] Pixels;
        private readonly int _maxWidth = 300;
        private readonly int _maxHeight = 300;
        public bool _sobelFilterActivated;
        public bool SobelFilterActivated
        {
            get { return _sobelFilterActivated; }
            set
            {
                _sobelFilterActivated = value;
                if(value)
                    ApplySobelFilter();
            }
        }
        public double Threshold;
      
        public CP_ImageScanPicture(ImageScanWindow parentWindow, string imageFilePath)
        {
            InitializeComponent();
            ParentWindow = parentWindow;
            ImageToGridMethod(imageFilePath);
            Console.WriteLine(imageFilePath);
            Threshold = ParentWindow.CpImageScanControls.ContrastSlider.Value;
        }

        private ImageScanWindow ParentWindow { get; }

        private void ImageToGridMethod(string imageFilePath)
        {
            if (!File.Exists(imageFilePath))
            {
                throw new FileNotFoundException();
            }
            _theImage = ResizeIfNecessary(new Bitmap(imageFilePath), _maxWidth, _maxHeight);

            width = _theImage.Width;
            height = _theImage.Height;

            Pixels = ConvertImageToGrayscale(_theImage);

            CreateVisualRepresentation();
            //CreateGridFile(, Pixels, _theImage.Width, _theImage.Height);
            //CreateGridFile("lolo123.grid", "header1", "desc", 100, Pixels, width, height);
        }

        private int width { get; set; }
        private int height { get; set; }

        private void CreateVisualRepresentation()
        {
            double threshold = ParentWindow.CpImageScanControls.ContrastSlider.Value;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int[] coords = { x, y };
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle()
                    {
                        Height = 10,
                        Width = 10,
                        Tag = coords,
                        Fill =
                            Pixels[y, x] >= threshold
                                ? new SolidColorBrush(Colors.Yellow)
                                : new SolidColorBrush(Colors.Black)
                    };
                    BuildingBlockContainer.Children.Add(rect);
                }
            }
        }


        public void ContrastChanger(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Pixels += ParentWindow.CpImageScanControls.ContrastSlider
        }


        private static Bitmap ResizeIfNecessary(Bitmap theImage, double maxWidth, double maxHeight)
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

        private static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
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

        private static double[,] ConvertImageToGrayscale(Bitmap image)
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

        private void ApplySobelFilter()
        {
            double[,] localPixelSet = new double[height, width];
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double a = Pixels[y - 1, x - 1],
                        b = Pixels[y - 1, x],
                        c = Pixels[y - 1, x + 1],
                        d = Pixels[y, x - 1],
                        e = Pixels[y, x + 1],
                        f = Pixels[y + 1, x - 1],
                        g = Pixels[y + 1, x],
                        h = Pixels[y + 1, x + 1];


                    double edgeVertical = (f + 2 * g + h) -(a + 2*b + c);

                    double edgeHorizontal = (a + 2*d + f) - (c + 2 * e + h);

                    localPixelSet[y, x] = Math.Sqrt(Math.Pow(edgeVertical, 2) + Math.Pow(edgeHorizontal, 2));
                }
            }
            Pixels = localPixelSet;

            foreach (var rect in BuildingBlockContainer.Children)
            {
                if (rect is System.Windows.Shapes.Rectangle)
                {
                    System.Windows.Shapes.Rectangle rectangle = rect as System.Windows.Shapes.Rectangle;
                    int[] coords = rectangle.Tag as int[];
                    bool recolour = ParentWindow.CpImageScanPicture.Pixels[coords[1], coords[0]] >=
                                    ParentWindow.CpImageScanPicture.Threshold;
                    if(SobelFilterActivated)
                        rectangle.Fill = recolour ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                    else
                    {
                        rectangle.Fill = recolour ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
                    }
                }
            }

        }
        private void CalculateContrast_OnClick(object sender, RoutedEventArgs e)
        {
            
            foreach (var rect in BuildingBlockContainer.Children)
            {
                if (rect is System.Windows.Shapes.Rectangle)
                {
                    System.Windows.Shapes.Rectangle rectangle = rect as System.Windows.Shapes.Rectangle;
                    int[] coords = rectangle.Tag as int[];
                    if (coords != null)
                    {
                        bool recolour = ParentWindow.CpImageScanPicture.Pixels[coords[1], coords[0]] >=
                                   ParentWindow.CpImageScanPicture.Threshold;
                        rectangle.Fill = recolour ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                    }
                   
                }
            }
        }
            

        /// <summary>
        /// Creates a new .grid file based on the given input image.
        /// </summary>
        /// <param name="filePath">Output filepath</param>
        /// <param name="header">Header / title</param>
        /// <param name="description"></param>
        /// <param name="contrastTurnOver">The RGB value (0 - 255) where it differentiates between a wall or free. 100 seems to do the trick.</param>
        public void CreateGridFile(string filePath, string header, string description, int contrastTurnOver)
        {
            StringBuilder gridFileText = new StringBuilder();
            gridFileText.Append($"<Settings>{Environment.NewLine}<Width>{width}</Width>{Environment.NewLine}<Height>{height}</Height>{Environment.NewLine}<Header>{header}</Header>{Environment.NewLine}<Description>{description}</Description>{Environment.NewLine}</Settings>{Environment.NewLine}<Grid>{Environment.NewLine}");
            for (int x = 1; x < width; x++)
            {
                gridFileText.Append("<Row>");
                for (int y = 1; y < height; y++)
                {
                    gridFileText.Append(Pixels[y, x] < contrastTurnOver ? "3" : "0");
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

    }
}
