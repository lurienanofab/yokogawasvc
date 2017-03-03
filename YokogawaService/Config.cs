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
