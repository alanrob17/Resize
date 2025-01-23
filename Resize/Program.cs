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
            // string imagePath = @"E:\Alan-Phone";
            string imagePath = Directory.GetCurrentDirectory();

            List<string> imageList = new();

            await ImageData.GetImageListAsync(imageList, imagePath);

            var x = 1;
            Console.Write("Running");

            foreach (var image in imageList)
            {
                await ImageData.ResizeImageAsync(imagePath, image);

                if (x % 10 == 0)
                {
                    Console.Write(".");
                }
                x++;
            }

            Console.WriteLine("\n\nFinished.");
        }
    }
}
