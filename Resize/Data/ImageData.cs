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
            image = image.Replace(imagePath, string.Empty).TrimStart('\\');
            var tempImage = Path.GetFileNameWithoutExtension(image);
            tempImage = tempImage + "_r.jpg";
            string resizePath = Path.Combine(Directory.GetCurrentDirectory(), "resized");
            var newImage = resizePath + @"\" + tempImage;
            
            if (!Directory.Exists(Path.GetDirectoryName(newImage)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newImage));
            }

            await ResizeImageByWidthAsync(image, newImage);
        }

        public static async Task ResizeImageByWidthAsync(string inputImagePath, string outputImagePath)
        {
            using (var inputImage = Image.FromFile(inputImagePath))
            {
                // Calculate new width as 80% of the original width
                var newWidth = (int)(inputImage.Width * 0.8);

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

                        // Copy EXIF tags from the original image to the new image
                        foreach (var propertyItem in inputImage.PropertyItems)
                        {
                            newImage.SetPropertyItem(propertyItem);
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
