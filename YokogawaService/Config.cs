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

        public string ConnectionString
        {
            get { return _section.ConnectionString; }
        }

        public string FolderPath
        {
            get { return _section.FolderPath; }
        }

        public string HeaderPattern
        {
            get { return _section.HeaderPattern; }
        }
    }
}
