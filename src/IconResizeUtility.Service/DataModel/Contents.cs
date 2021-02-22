﻿using Newtonsoft.Json;

namespace IconResizeUtility.Service.DataModel
{
    public class Contents
    {
        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }

    }
}
