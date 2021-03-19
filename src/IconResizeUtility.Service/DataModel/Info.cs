using Newtonsoft.Json;

namespace IconResizeUtility.Service.DataModel
{
    public class Info
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }
    }
}
