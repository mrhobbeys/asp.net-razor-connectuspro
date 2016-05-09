using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Windows.Forms;
using _800PlumberServerWare;
using log4net.Config;

namespace ConnectusAppServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var process = Process.GetCurrentProcess();
            var running = Process.GetProcessesByName(process.ProcessName).Any(p => GetProcessOwner(p.Id) == GetProcessOwner(process.Id) && p.Id != process.Id);

            if (running)
            {
                MessageBox.Show("This process is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (args.Length == 0 || args[0].Split('=').First() != "mode")
            {
                RunInteractiveMode();
                return;
            }

            var mode = args[0].Split('=').Last();

            switch (mode)
            {
                case "service":
                    {
                        var toRun = new[] { new ConnectusAppServer() };
                        ServiceBase.Run(toRun);
                    }
                    break;
                case "interactive":
                    {
                        RunInteractiveMode();
                    }
                    break;
            }
        }

        static string GetProcessOwner(int processId)
        {
            var name = "NO OWNER";
            var obj = new ManagementObjectSearcher(string.Format("SELECT * FROM Win32_Process WHERE ProcessID = {0}", processId))
                            .Get().OfType<ManagementObject>().SingleOrDefault();

            if (obj != null)
            {
                var argList = new[] { string.Empty };
                var returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0) name = argList[0];
            }

            return name;
        }

        private static void RunInteractiveMode()
        {
            var frm = new frm_MainMenu();
            frm.ShowDialog();
        }
    }
}
