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
    // This class is used to represet data from local physical files as well as remote imported data.
    // for local files it is created directly from the file data. For remote data it is created from
    // the FileImportData class. The only difference between this class and FileImportData is that
    // FileImportData has an ObjectId property for Mongo.

    public class YokogawaFileData
    {
        public int FileIndex { get; set; }
        public int LineIndex { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Header { get; set; }
        public double Value { get; set; }
    }
}
