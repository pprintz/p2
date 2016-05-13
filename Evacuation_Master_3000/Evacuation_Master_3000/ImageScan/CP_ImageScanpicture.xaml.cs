using System;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Drawing;

namespace Evacuation_Master_3000.ImageScan
{
    /// <summary>
    /// Interaction logic for CP_ImageScanPicture.xaml
    /// </summary>
    public partial class CP_ImageScanPicture
    {

        private static Bitmap _theImage;
        private double[,] _pixelsRegular;
        private double[,] _pixelsSobel;
        private double[,] _pixelsCurrentlyActive;
        private readonly int _maxWidth = 300;
        private readonly int _maxHeight = 300;
        private bool _firstTimeDrawing = true;
        private bool _sobelFilterActivated;
        private ImageScanWindow ParentWindow { get; }
        public double ContrastThreshold;
        public bool SobelFilterActivated
        {
            get { return _sobelFilterActivated; }
            set
            {
                _sobelFilterActivated = value;
                _pixelsCurrentlyActive = value ? _pixelsSobel : _pixelsRegular;
            }
        }
        private int ImageWidth { get; set; }
        private int ImageHeight { get; set; }

        public CP_ImageScanPicture(ImageScanWindow parentWindow, string imageFilePath)
        {
            InitializeComponent();
            ParentWindow = parentWindow;
            CalculateAndSetupVisualRepresentation(imageFilePath);
            ContrastThreshold = ParentWindow.CpImageScanControls.ContrastSlider.Value;
        }

        

        private void CalculateAndSetupVisualRepresentation(string imageFilePath)
        {
            if (!File.Exists(imageFilePath))
            {
                throw new FileNotFoundException();
            }
            _theImage = ImageScanningTools.ResizeIfNecessary(new Bitmap(imageFilePath), _maxWidth, _maxHeight);

            ImageWidth = _theImage.Width;
            ImageHeight = _theImage.Height;

            _pixelsRegular = ImageScanningTools.ConvertImageToGrayscale(_theImage);
            _pixelsSobel = ImageScanningTools.ApplySobelFilter(_pixelsRegular, ImageWidth, ImageHeight);
            _pixelsCurrentlyActive = _pixelsRegular;
            CreateOrUpdateVisualRepresentation();
        }


        public void CreateOrUpdateVisualRepresentation()
        {
            if (_firstTimeDrawing)
            {
                for (int y = 0; y < ImageHeight; y++)
                {
                    for (int x = 0; x < ImageWidth; x++)
                    {
                        int[] coordinates = {x, y};

                        System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle()
                        {
                            Height = 10,
                            Width = 10,
                            Tag = coordinates,
                            Fill = DecideColor(_pixelsCurrentlyActive[y, x])
                        };
                        BuildingBlockContainer.Children.Add(rect);
                    }
                }
                _firstTimeDrawing = false;
            }
            else
            {
                foreach (var rect in BuildingBlockContainer.Children)
                {
                    if (rect is System.Windows.Shapes.Rectangle)
                    {
                        System.Windows.Shapes.Rectangle rectangle = rect as System.Windows.Shapes.Rectangle;
                        int[] coordinates = rectangle.Tag as int[];
                        if (coordinates != null)
                        {
                            rectangle.Fill = DecideColor(_pixelsCurrentlyActive[coordinates[1], coordinates[0]]);
                        }
                        else
                        {
                            throw new GeneralInternalException("The rectangles coordinates could not be found.");
                        }
                        
                    }
                }
            }
        }
        

        private SolidColorBrush DecideColor(double pixelValue)
        {
            if (SobelFilterActivated)
            {
                return pixelValue >= ContrastThreshold
                    ? new SolidColorBrush(Colors.Black)
                    : new SolidColorBrush(Colors.White);
            }
            // The Sobel filter gives a high value if there is a big contrast.
            return pixelValue >= ContrastThreshold
                ? new SolidColorBrush(Colors.White)
                : new SolidColorBrush(Colors.Black);
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
            gridFileText.Append($"<Settings>{Environment.NewLine}<Width>{ImageWidth}</Width>{Environment.NewLine}<Height>{ImageHeight}</Height>{Environment.NewLine}<Header>{header}</Header>{Environment.NewLine}<Description>{description}</Description>{Environment.NewLine}</Settings>{Environment.NewLine}<Grid>{Environment.NewLine}");
            for (int x = 1; x < ImageWidth; x++)
            {
                gridFileText.Append("<Row>");
                for (int y = 1; y < ImageHeight; y++)
                {
                    gridFileText.Append(_pixelsRegular[y, x] < contrastTurnOver ? "3" : "0");
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
                throw new GeneralInternalException(@"Something went wrong with creating the gridfile: " + e);
            }
        }

    }
}
