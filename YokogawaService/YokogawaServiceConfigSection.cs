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

        // default: every 15 minutes
        [ConfigurationProperty("minuteGranularity", IsRequired = false, DefaultValue = 15)]
        public int MinuteGranularity
        {
            get { return (int)this["minuteGranularity"]; }
            set { this["minuteGranularity"] = value; }
        }

        // default: every hour
        [ConfigurationProperty("hourGranularity", IsRequired = false, DefaultValue = 1)]
        public int HourGranularity
        {
            get { return (int)this["hourGranularity"]; }
            set { this["hourGranularity"] = value; }
        }

        [ConfigurationProperty("showSql", IsRequired = false, DefaultValue = false)]
        public bool ShowSql
        {
            get { return (bool)this["showSql"]; }
            set { this["showSql"] = value; }
        }
    }
}
