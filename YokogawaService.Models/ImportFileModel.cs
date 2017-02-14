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

using System;

namespace YokogawaService.Models
{
    public enum SampleGranularity
    {
        None = 0,
        Minute = 1,
        Hour = 2,
        Day = 3
    }

    public class ImportFileModel
    {
        public int Index { get; set; }
        public string FilePath { get; set; }
        public DateTime ImportDate { get; set; }
        public int LineCount { get; set; }
        public SampleGranularity Granularity { get; set; }
    }
}
