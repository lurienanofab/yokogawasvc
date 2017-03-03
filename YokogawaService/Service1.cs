using Microsoft.Owin.Hosting;
using System;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using YokogawaService.Models;

namespace YokogawaService
{
    public partial class Service1 : ServiceBase
    {
        public static readonly string InstallServiceName = "YokogawaService";

        private IDisposable _webapp;

        private Timer _timer;

        private static object _lockerObject = new object();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _webapp = WebApp.Start<Startup>(Config.Current.ServiceUrl);

            if (!Directory.Exists(Config.Current.FolderPath))
                Directory.CreateDirectory(Config.Current.FolderPath);

            bool startTimer = true;

            if (startTimer)
                _timer = new Timer(x => TimerElapsed(x), null, 0, 1000);
        }

        private void TimerElapsed(object stateInfo)
        {
            if (Monitor.TryEnter(_lockerObject))
            {
                try
                {
                    using (var uow = DataManager.Current.StartUnitOfWork())
                    {
                        lock (ImportIndex.Instance)
                        {
                            YokogawaFile yokoFile = null;

                            if (ImportIndex.Instance.Value == -1)
                            {
                                var importFileIndex = uow.GetMaxFileIndex().GetValueOrDefault(-1);

                                if (importFileIndex == -1)
                                {
                                    yokoFile = FileUtility.GetFirstFile();

                                    if (yokoFile != null)
                                        ImportIndex.Instance.Value = yokoFile.Index;
                                }
                                else
                                    ImportIndex.Instance.Value = importFileIndex + 1;
                            }

                            if (ImportIndex.Instance.Value == -1)
                            {
                                Program.Log("[timer] nothing to import, index is -1");
                                return;
                            }

                            if (yokoFile == null)
                                yokoFile = FileUtility.GetFile(ImportIndex.Instance.Value);

                            if (yokoFile != null)
                            {
                                Program.Log("[timer] found file: {0}", Path.GetFileName(yokoFile.FilePath));

                                var importFile = uow.ImportFile(yokoFile);

                                if (importFile != null)
                                {
                                    if (importFile.LineCount == 0)
                                    {
                                        // happens when there is no data found in the file
                                        Program.Log("[timer] no data found");
                                    }
                                    else
                                    {
                                        Program.Log("[timer] imported: {0} lines at {1:yyyy-MM-dd HH:mm:ss}", importFile.LineCount, importFile.ImportDate.ToLocalTime());
                                    }
                                }
                                else
                                {
                                    Program.Log("[timer] null result from ImportManager.ImportFile");
                                }

                                ImportIndex.Instance.Increment();

                                Program.Log("[timer] next index: {0}", ImportIndex.Instance.Value);
                            }
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_lockerObject);
                }
            }
        }

        protected override void OnStop()
        {
            _webapp.Dispose();
            _timer.Dispose();
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
