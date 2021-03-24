namespace IconResizeUtility.Service
{
    public interface IImageRenamer
    {
        string ConvertToValidIconName(string iconName);
        string AddPostfix(string iconName, string postfix);
        string AddPrefix(string iconName, string pefix);
    }
}
