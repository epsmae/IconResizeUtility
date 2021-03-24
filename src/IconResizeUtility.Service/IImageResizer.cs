namespace IconResizeUtility.Service
{
    public interface IImageResizer
    {
        /// <summary>
        /// Resize an image
        /// </summary>
        /// <param name="srcImagePath"></param>
        /// <param name="dstImagePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="hexColor">Hex color code e.g. "#335566FF"</param>
        void Resize(string srcImagePath, string dstImagePath, int width, int height, string hexColor = null);

        ImageInfo GetInfo(string srcImagePath);
    }
}
