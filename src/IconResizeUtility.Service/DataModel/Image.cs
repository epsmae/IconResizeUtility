using Newtonsoft.Json;

namespace IconResizeUtility.Service.DataModel
{
    public class Image
    {
        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("idiom")]
        public string Idiom { get; set; }

        [JsonProperty("scale")]
        public string Scale { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }
    }
}
