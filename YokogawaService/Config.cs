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
using System.Configuration;

namespace YokogawaService
{
    public class Config
    {
        public static Config Current { get; }

        static Config()
        {
            Current = new Config();
        }

        private readonly YokogawaServiceConfigSection _section;

        private Config()
        {
            _section = (YokogawaServiceConfigSection)ConfigurationManager.GetSection("yokogawasvc");
        }

        public string ServiceUrl
        {
            get { return _section.ServiceUrl; }
        }

        public string FolderPath
        {
            get { return _section.FolderPath; }
        }

        public string HeaderPattern
        {
            get { return _section.HeaderPattern; }
        }

        public int HourGranularity
        {
            get
            {
                int result = _section.HourGranularity;

                if (result <= 0)
                    throw new Exception("HourGranularity must be between 1 and 24");

                return result;
            }
        }

        public int MinuteGranularity
        {
            get
            {
                int result = _section.MinuteGranularity;

                if (result <= 0)
                    throw new Exception("MinuteGranularity must be between 1 and 60");

                return result;
            }
        }

        public bool ShowSql
        {
            get { return _section.ShowSql; }
        }
    }
}
