using Newtonsoft.Json;
using System.Collections.Generic;

namespace YokogawaService.Models
{
    public class ReportDataset
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("data")]
        public IList<double> Data { get; set; }

        [JsonProperty("borderColor")]
        public string BorderColor { get; set; }

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("pointBorderColor")]
        public string PointBorderColor { get; set; }

        [JsonProperty("pointBackgroundColor")]
        public string PointBackgroundColor { get; set; }

        [JsonProperty("fill")]
        public bool Fill { get; set; }

        [JsonProperty("unitCost")]
        public double UnitCost { get; set; }
    }
}
