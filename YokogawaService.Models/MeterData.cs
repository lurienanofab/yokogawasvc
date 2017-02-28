﻿/*
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
    public class MeterData
    {
        public virtual int MeterDataID { get; set; }

        public virtual int FileIndex { get; set; }

        public virtual int LineIndex { get; set; }

        public virtual string Header { get; set; }

        public virtual DateTime TimeStamp { get; set; }

        public virtual double Value { get; set; }
    }
}
