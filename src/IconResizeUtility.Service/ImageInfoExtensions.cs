namespace IconResizeUtility.Service
{
    public static class ImageInfoExtensions
    {
        /// <summary>
        /// Calculates the new with and height for a given factor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="factor"></param>
        public static void ScaleDownByFactor(this ImageInfo info, int factor)
        {
            info.Height = info.Height / factor;
            info.Width = info.Width / factor;
        }

        /// <summary>
        /// Calculates the new with and height for a given width
        /// Preserves the aspect ratio
        /// </summary>
        /// <param name="info"></param>
        /// <param name="width"></param>
        public static void ScaleByWidth(this ImageInfo info, int width)
        {
            double scaleFactor = (double)info.Width / width;
            info.Height = (int) (info.Height / scaleFactor);
            info.Width = width;
        }
    }
}
