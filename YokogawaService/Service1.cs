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

using Microsoft.Owin.Hosting;
using System;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace YokogawaService
{
    public partial class Service1 : ServiceBase
    {
        public static readonly string InstallServiceName = "AzureSync";

        private IDisposable _webapp;

        private Timer _timer;

        private static object _lockerObject = new object();

        private static int _nextIndex;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _webapp = WebApp.Start<Startup>(Config.Current.ServiceUrl);

            if (!Directory.Exists(Config.Current.FolderPath))
                Directory.CreateDirectory(Config.Current.FolderPath);

            var filesController = new Controllers.FilesController();
            _nextIndex = filesController.GetNextIndex();
            Program.Log("[timer] next index: {0}", _nextIndex);

            _timer = new Timer(new TimerCallback(TimeElapsed), null, 0, 10000);
        }

        private void TimeElapsed(object stateInfo)
        {
            if (Monitor.TryEnter(_lockerObject))
            {
                try
                {
                    var yokoFile = FileUtility.GetFile(_nextIndex);

                    if (yokoFile != null)
                    {
                        Program.Log("[timer] next file: {0}", Path.GetFileName(yokoFile.FilePath));
                        var filesController = new Controllers.FilesController();
                        var model = filesController.ImportNextFile();

                        if (model.LineCount == 0)
                        {
                            // happens when there is no data found in the file
                            Program.Log("[timer] no data found");
                        }
                        else
                        {
                            Program.Log("[timer] imported: {0} lines", model.LineCount);
                        }

                        _nextIndex += 1;

                        Program.Log("[timer] next index: {0}", _nextIndex);
                    }
                }
                finally
                {
                    Monitor.Exit(_lockerObject);
                }
            }
        }

        public static void SetNextIndex(int value)
        {
            lock (_lockerObject)
            {
                _nextIndex = value;
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
