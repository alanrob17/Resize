using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
// using System.Text;
using System.Threading.Tasks;
// using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Numerics;
using System.Reflection.Metadata;
// using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

namespace Resize.Data
{
    internal class ImageData
    {
        internal static void ResizeImage(string imagePath, string image)
        {
            var newImage = @"E:\Resize" + image.Replace(@"E:\Alan-Phone", string.Empty);

            ResizeImageByWidth(image, newImage);
        }

        public static void ResizeImageByWidth(string inputImagePath, string outputImagePath)
        {
            var newWidth = 0;

            using (var inputImage = Image.FromFile(inputImagePath))
            {
                // Read the EXIF orientation tag
                int exifOrientationTag = 274; // EXIF orientation tag
                if (inputImage.PropertyIdList.Contains(exifOrientationTag))
                {
                    var orientation = (int)inputImage.GetPropertyItem(exifOrientationTag).Value[0];
                    if (orientation == 6)
                    {
                        // Rotate 90 degrees clockwise for EXIF orientation 6
                        inputImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        newWidth = 500;
                    }
                    else
                    {
                        newWidth = 800;
                    }
                    // Handle other orientation values as needed
                }
                else
                {
                    // Default width if EXIF orientation tag is not present
                    newWidth = 800;
                }

                if (newWidth > 0)
                {
                    int newHeight = (int)Math.Round(inputImage.Height * (newWidth / (double)inputImage.Width));
                    if (newHeight > 0)
                    {
                        var newImage = new Bitmap(newWidth, newHeight);

                        using (var graphics = Graphics.FromImage(newImage))
                        {
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage(inputImage, new Rectangle(0, 0, newWidth, newHeight));
                        }

                        newImage.Save(outputImagePath, ImageFormat.Jpeg); // You can choose the desired image format.
                    }
                    else
                    {
                        throw new ArgumentException("Calculated new height is not valid.");
                    }
                }
                else
                {
                    throw new ArgumentException("Calculated new width is not valid.");
                }
            }
        }

        private static bool IsPortrait(Image image)
        {
            return image.Width > image.Height;
        }
        internal static List<string> GetImageList(List<string> imageList, string folder)
        {
            var dir = new DirectoryInfo(folder);
            GetImageFiles(dir, imageList);

            return imageList;
        }

        private static void GetImageFiles(DirectoryInfo d, List<string> imageList)
        {
            var files = d.GetFiles("*.*");

            foreach (FileInfo file in files)
            {
                var fileName = file.FullName;

                var newDirectory = Path.GetDirectoryName(fileName);
                var dirName = d.FullName;

                if (Path.GetExtension(fileName.ToLowerInvariant()) == ".jpg")
                {
                    imageList.Add(fileName);
                }
            }

            // get sub-folders for the current directory
            var dirs = d.GetDirectories("*.*");

            // recurse
            foreach (DirectoryInfo dir in dirs)
            {
                // Console.WriteLine("--------->> {0} ", dir.Name);
                GetImageFiles(dir, imageList);
            }
        }
    }
}
