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
                        using (FileStream dstStream = File.OpenWrite(dstImagePath))
                        {
                            resizedBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(dstStream);
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
