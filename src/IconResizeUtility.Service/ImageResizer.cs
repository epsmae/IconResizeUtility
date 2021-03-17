using System.IO;
using SkiaSharp;

namespace IconResizeUtility.Service
{
    public class ImageResizer
    {
        public void Resize(string srcImagePath, string dstImagePath, int width, int height)
        {
            using (FileStream srcStream = File.OpenRead(srcImagePath))
            {
                using (SKBitmap srcBitmap = SKBitmap.Decode(srcStream))
                {
                    using (SKBitmap resizedBitmap = srcBitmap.Resize(new SKSizeI(width, height), SKFilterQuality.Medium))
                    {
                        SKBitmap bmp = new SKBitmap(new SKImageInfo(resizedBitmap.Width, resizedBitmap.Height, resizedBitmap.ColorType, resizedBitmap.AlphaType, resizedBitmap.ColorSpace));

                        SKCanvas canvas = new SKCanvas(bmp);
                        
                        using (FileStream dstStream = File.OpenWrite(dstImagePath))
                        {
                            SKPaint paint = new SKPaint();
                            paint.Color = new SKColor(0, 150, 0);
                            
                            canvas.DrawBitmap(resizedBitmap, 0, 0, paint);

                            bmp.Encode(SKEncodedImageFormat.Png, 100).SaveTo(dstStream);
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
    }
}
