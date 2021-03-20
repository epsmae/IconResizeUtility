using System;
using System.IO;
using NUnit.Framework;
using SkiaSharp;

namespace IconResizeUtility.TestInfrastructure
{
    public class TestColorHelper
    {
        /// <summary>
        /// Computes the average color transparent images are ignores
        /// </summary>
        /// <param name="srcImagePath"></param>
        /// <returns></returns>
        public static SKColor GetAverageColor(string srcImagePath)
        {
            using (FileStream srcStream = File.OpenRead(srcImagePath))
            {
                using (SKBitmap srcBitmap = SKBitmap.Decode(srcStream))
                {
                    return GetAverageColor(srcBitmap);
                }
            }
        }

        /// <summary>
        /// Computes the average color of a bitmap, transparent pixels are ignored
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static SKColor GetAverageColor(SKBitmap image)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            int alpha = 0;

            int pixelCount = 0;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    SKColor pixelColor = image.GetPixel(x, y);
                    
                    if (pixelColor.Alpha > 0)
                    {
                        red += pixelColor.Red;
                        green += pixelColor.Green;
                        blue += pixelColor.Blue;
                        alpha += pixelColor.Alpha;

                        pixelCount++;
                    }
                }
            }

            if (pixelCount > 0)
            {
                red /= pixelCount;
                green /= pixelCount;
                blue /= pixelCount;
                alpha /= pixelCount;
            }
            
            return new SKColor((byte)red, (byte)green, (byte)blue, (byte)alpha);
        }

        public static void AssertSameColor(in SKColor expectedColoer, in SKColor actualColor)
        {
            const int tolerance = 5;
            EnsureInTolerance(expectedColoer.Red, actualColor.Red, tolerance);
            EnsureInTolerance(expectedColoer.Green, actualColor.Green, tolerance);
            EnsureInTolerance(expectedColoer.Blue, actualColor.Blue, tolerance);
            EnsureInTolerance(expectedColoer.Alpha, actualColor.Alpha, tolerance);
        }

        private static void EnsureInTolerance(byte expected, byte actual, int tolerance)
        {
            Assert.True(Math.Abs(expected - actual) <= tolerance);
        }
    }
}
