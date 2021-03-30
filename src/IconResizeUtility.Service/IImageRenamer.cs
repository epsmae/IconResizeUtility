namespace IconResizeUtility.Service
{
    public interface IImageRenamer
    {

        string ConvertToValidIconName(string iconName);
        
        /// <summary>
        /// Adds a postfix to the name respecting the file extension
        /// </summary>
        /// <param name="iconName"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        string AddPostfix(string iconName, string postfix);
        
        /// <summary>
        /// Adds a prefix to the icon name
        /// </summary>
        /// <param name="iconName"></param>
        /// <param name="pefix"></param>
        /// <returns></returns>
        string AddPrefix(string iconName, string pefix);
    }
}
