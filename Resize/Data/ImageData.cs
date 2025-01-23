using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace Resize.Data
{
    internal class ImageData
    {
        internal static async Task ResizeImageAsync(string imagePath, string image)
        {
            var newImage = @"E:\Resize" + image.Replace(@"E:\Alan-Phone", string.Empty);

            await ResizeImageByWidthAsync(image, newImage);
        }

        public static async Task ResizeImageByWidthAsync(string inputImagePath, string outputImagePath)
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

                        await Task.Run(() => newImage.Save(outputImagePath, ImageFormat.Jpeg)); // You can choose the desired image format.
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

        internal static async Task<List<string>> GetImageListAsync(List<string> imageList, string folder)
        {
            var dir = new DirectoryInfo(folder);
            await GetImageFilesAsync(dir, imageList);

            return imageList;
        }

        private static async Task GetImageFilesAsync(DirectoryInfo d, List<string> imageList)
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
                await GetImageFilesAsync(dir, imageList);
            }
        }
    }
}
