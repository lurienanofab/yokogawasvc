/*
   Copyright 2017 University of Michigan

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License. 
*/

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
