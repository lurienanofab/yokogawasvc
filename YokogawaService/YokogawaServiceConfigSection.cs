using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [ConfigurationProperty("syncFolder", IsRequired = true)]
        public string SyncFolder
        {
            get { return (string)this["syncFolder"]; }
            set { this["syncFolder"] = value; }
        }
    }
}
