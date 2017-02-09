using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.Reflection;
using Microsoft.Owin.Hosting;
using YokogawaService.Controllers;

namespace YokogawaService
{
    public partial class Service1 : ServiceBase
    {
        public static readonly string InstallServiceName = "AzureSync";

        private IDisposable _webapp;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _webapp = WebApp.Start<Startup>(Config.Current.ServiceUrl);
        }

        protected override void OnStop()
        {
        }

        public void Start(string[] args)
        {
            Program.ConsoleWriteLine("endpoint: {0}", Config.Current.ServiceUrl);
            Program.ConsoleWriteLine("Press any key to exit.");
            OnStart(args);
        }

        public static void InstallService()
        {
            if (IsServiceInstalled())
            {
                UninstallService();
            }

            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
        }

        public static void UninstallService()
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
        }

        public static bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == InstallServiceName);
        }
    }
}
