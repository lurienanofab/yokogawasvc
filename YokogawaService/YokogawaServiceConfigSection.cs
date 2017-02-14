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
    public class YokogawaServiceConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("serviceUrl", IsRequired = true)]
        public string ServiceUrl
        {
            get { return (string)this["serviceUrl"]; }
            set { this["serviceUrl"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        [ConfigurationProperty("folderPath", IsRequired = true)]
        public string FolderPath
        {
            get { return (string)this["folderPath"]; }
            set { this["folderPath"] = value; }
        }

        [ConfigurationProperty("headerPattern", IsRequired = true)]
        public string HeaderPattern
        {
            get { return (string)this["headerPattern"]; }
            set { this["headerPattern"] = value; }
        }
    }
}
