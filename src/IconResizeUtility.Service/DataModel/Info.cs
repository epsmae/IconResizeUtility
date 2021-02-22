using System;
using Newtonsoft.Json;

namespace IconResizeUtility.Service.DataModel
{
    public class Info
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("template-rendering-intent")]
        public string TemplateRenderingIntent { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
