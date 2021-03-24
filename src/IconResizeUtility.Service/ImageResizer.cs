using System.IO;
using SkiaSharp;

namespace IconResizeUtility.Service
{
    public class ImageResizer : IImageResizer
    {
        /// <summary>
        /// Resize an image
        /// </summary>
        /// <param name="srcImagePath"></param>
        /// <param name="dstImagePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="hexColor">Hex color code e.g. "#335566FF"</param>
        public void Resize(string srcImagePath, string dstImagePath, int width, int height, string hexColor = null)
        {
            using (FileStream srcStream = File.OpenRead(srcImagePath))
            {
                using (SKBitmap srcBitmap = SKBitmap.Decode(srcStream))
                {
                    using (SKBitmap resizedBitmap = srcBitmap.Resize(new SKSizeI(width, height), SKFilterQuality.Medium))
                    {
                        if (string.IsNullOrEmpty(hexColor))
                        {
                            SaveImage(dstImagePath, resizedBitmap);
                        }
                        else
                        {
                            using (SKBitmap tintedBitmap = TintBitmap(resizedBitmap, hexColor))
                            {
                                SaveImage(dstImagePath, tintedBitmap);
                            }
                        }
                    }
                }
            }
        }
        
        public ImageInfo GetInfo(string srcImagePath)
        {
            using (FileStream srcStream = File.OpenRead(srcImagePath))
            {
                using (SKBitmap srcBitmap = SKBitmap.Decode(srcStream))
                {
                    return new ImageInfo
                    {
                        Height = srcBitmap.Height,
                        Width = srcBitmap.Width
                    };
                }
            }
        }

        private static void SaveImage(string dstImagePath, SKBitmap resizedBitmap)
        {
            using (FileStream dstStream = File.OpenWrite(dstImagePath))
            {
                resizedBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(dstStream);
            }
        }

        public SKBitmap TintBitmap(SKBitmap src, string hexColor)
        {
            SKBitmap result = new SKBitmap(new SKImageInfo(src.Width, src.Height, src.ColorType, src.AlphaType, src.ColorSpace));

            using (SKCanvas canvas = new SKCanvas(result))
            {
                using (SKPaint paint = new SKPaint())
                {
                    canvas.DrawColor(SKColors.Transparent);

                    SKColor color = SKColor.Parse(hexColor);

                    paint.ColorFilter = SKColorFilter.CreateBlendMode(color, SKBlendMode.SrcIn);

                    canvas.DrawBitmap(src, 0, 0, paint);
                }
            }
            return result;
        }
    }
}
