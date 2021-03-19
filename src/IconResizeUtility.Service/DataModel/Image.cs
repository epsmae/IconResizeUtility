using Newtonsoft.Json;

namespace IconResizeUtility.Service.DataModel
{
    public class Image
    {
        [JsonProperty("appearances")]
        public string[] Appearances { get; set; }

        [JsonProperty("scale")]
        public string Scale { get; set; }

        [JsonProperty("idiom")]
        public string Idiom { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }
    }
}
