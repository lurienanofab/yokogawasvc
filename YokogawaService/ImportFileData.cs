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

using MongoDB.Bson;
using System;

namespace YokogawaService
{
    public class ImportFileData
    {
        public ObjectId Id { get; set; }

        public int FileIndex { get; set; }

        public int LineIndex { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Header { get; set; }

        public double Value { get; set; }

        public DateTime ImportDate { get; set; }
    }
}
