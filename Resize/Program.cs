using Resize.Data;

namespace Resize
{
    internal class Program
    {
        // NuGet - System.Drawing.Common
        static void Main(string[] args)
        {
            string imagePath = @"E:\Alan-Phone";

            List<string> imageList = new();

            ImageData.GetImageList(imageList, imagePath);

            foreach (var image in imageList)
            {
               ImageData.ResizeImage(imagePath, image);
            }
        }
    }
}