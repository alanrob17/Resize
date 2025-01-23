using Resize.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resize
{
    internal class Program
    {
        // NuGet - System.Drawing.Common
        public static async Task Main(string[] args)
        {
            string imagePath = @"E:\Alan-Phone";

            List<string> imageList = new();

            await ImageData.GetImageListAsync(imageList, imagePath);

            foreach (var image in imageList)
            {
                await ImageData.ResizeImageAsync(imagePath, image);
                Console.Write(".");
            }

            Console.WriteLine("\nFinished.");
        }
    }
}
