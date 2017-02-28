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

namespace YokogawaService.Models
{
    public class Report
    {
        public virtual int ReportID { get; set; }
        public virtual string ReportType { get; set; }
        public virtual string ReportName { get; set; }
        public virtual string Header { get; set; }
        public virtual double UnitCost { get; set; }
        public virtual string BorderColor { get; set; }
        public virtual string BackgroundColor { get; set; }
        public virtual string PointBorderColor { get; set; }
        public virtual string PointBackgroundColor { get; set; }
        public virtual bool Active { get; set; }
    }
}
