using Newtonsoft.Json;
using System.Collections.Generic;

namespace YokogawaService.Models
{

    public class ReportModel
    {
        [JsonProperty("labels")]
        public IEnumerable<string> Labels { get; set; }

        [JsonProperty("datasets")]
        public IEnumerable<ReportDataset> Datasets { get; set; }
    }
}
