//using System;
//using System.Diagnostics;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Evacuation_Master_3000
//{
//    class ImageProgram
//    {
//        static void Main()
//        {


//            /*
//            Image theImage = Image.FromFile(inputFilePath);
//            theImage = GeneralImage.ResizeImage(theImage, 102, 63);
//            theImage.Save(outputFilePath);*/

//            //GeneralImage img = new GeneralImage(inputFilePath, 300, 300);

//            //img.ExtractPixels();
//            //img.CalculateEdginessAndChangeRGBValues();
//            ////img.WriteImageToFile(outputFilePath);

//            //img.CreateGridFile(outputFilePath, "SimpleBuildPlan", "Ok", 100);
//            try
//            {
//                Stopwatch sw = Stopwatch.StartNew();
//                string theFilePath = @"../../imageFiles/";
//                Console.WriteLine("Input filename:");
//                string inputFilePath = theFilePath + Console.ReadLine();
//                Console.WriteLine("Output filename:");
//                string outputFilePath = theFilePath + Console.ReadLine();

//                ImageToGrid imgGrid = new ImageToGrid(inputFilePath, outputFilePath, "TestyHeader", "Desc", 100, 320, 180, true);
//                Console.WriteLine("Thank you. Just wait a minute while our robots are looking at your image.");

//                sw.Stop();
//                Console.WriteLine("Robots spend a bit of time." + sw.Elapsed + " to be exact.");

//            }
//            catch (FileNotFoundException e)
//            {
//                Console.WriteLine($"File:{e.FileName} could not be found.");
//            }
//            finally
//            {
//                Console.ReadKey();
//            }




//        }

//    }
//}
