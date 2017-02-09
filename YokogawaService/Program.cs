using System;
using System.ServiceProcess;

namespace YokogawaService
{
    static class Program
    {
        private static bool _consoleMode = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                for (int ii = 0; ii < args.Length; ii++)
                {
                    switch (args[ii].ToLower())
                    {
                        case "-i":
                        case "--install":
                            Service1.InstallService();
                            return;
                        case "-u":
                        case "--uninstall":
                            Service1.UninstallService();
                            return;
                        case "-c":
                        case "--console":
                            _consoleMode = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            var service = new Service1();

            if (_consoleMode)
            {
                Console.Clear();
                Console.Title = Service1.InstallServiceName;
                service.Start(null);
                Console.ReadKey(true);
                service.Stop();
                Environment.Exit(0);
            }
            else
            {
                ServiceBase.Run(new[] { service });
            }
        }

        public static void ConsoleWriteLine(string msg, params object[] args)
        {
            if (_consoleMode)
                Console.WriteLine(msg, args);
        }

        public static void Log(string msg, params object[] args)
        {
            ConsoleWriteLine("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, string.Format(msg, args));
        }
    }
}
