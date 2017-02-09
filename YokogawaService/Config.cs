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

        public string SyncFolder
        {
            get { return _section.SyncFolder; }
        }
    }
}
