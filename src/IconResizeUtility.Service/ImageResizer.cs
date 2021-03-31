using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace IconResizeUtility.Service
{
    public class ImageResizer : IImageResizer
    {
        public ImageResizer()
        {
            _cachedItems = new List<CacheItem>();
        }

        private bool _useCache;
        

        private IList<CacheItem> _cachedItems;

        public bool UseCache
        {
            get => _useCache;
            set => _useCache = value;
        }

        public IList<CacheItem> CachedItems => _cachedItems;


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
            //CacheItem cachedItem = TryGetCachedItem(srcImagePath, width, height, hexColor);

            if (_useCache && TryGetCachedItem(srcImagePath, width, height, hexColor, out CacheItem cachedItem))
            {
                File.Copy(cachedItem.Path, dstImagePath, true);
            }
            else
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

                if (_useCache)
                {
                    _cachedItems.Add(new CacheItem
                    {
                        Name = Path.GetFileName(srcImagePath),
                        HexColor = hexColor,
                        Width = width,
                        Height = height,
                        Path = dstImagePath
                    });

                }
            }
        }

        private bool TryGetCachedItem(string srcImagePath, int width, int height, string hexColor, out CacheItem item)
        {
            item = _cachedItems.FirstOrDefault(item => item.Name == Path.GetFileName(srcImagePath)
                                                 && item.Width == width
                                                 && item.Height == height
                                                 && item.HexColor == hexColor);

            return item != null;
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
