using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using _800PlumberServerWare;

namespace ConnectusAppServer
{
    public partial class ConnectusAppServer : ServiceBase
    {
        private const string LogName = "Connectus App Server";
        private Engine _engine;

        public ConnectusAppServer()
        {
            InitializeComponent();
            var envName = string.Format("Connectus{0}", ConfigurationManager.AppSettings["Environment"]);

            if (!EventLog.SourceExists(envName))
                EventLog.CreateEventSource(envName, LogName);

            eventLogger.Source = envName;
            eventLogger.Log = LogName;

        }

        protected override void OnShutdown()
        {
            Cleanup();
        }
        
        protected override void OnStart(string[] args)
        {
            if (ConfigurationManager.AppSettings["DebugMode"] == "true")
                Debugger.Launch();

            _engine = Engine.Instance;
            eventLogger.WriteEntry("Service started.");
        }

        protected override void OnPause()
        {
            if (_engine != null)
                _engine.Pause(true);

            eventLogger.WriteEntry("Service paused.");
        }

        protected override void OnContinue()
        {
            if (_engine != null)
                _engine.Pause(false);

            eventLogger.WriteEntry("Service resumed.");
        }

        protected override void OnStop()
        {
            const int maxCount = 20;
            var count = 0;
            var msg = "Waiting to shut down...";
            
            CommonFunctions.Log(string.Empty, msg, null, LogLevel.Info);
            eventLogger.WriteEntry(msg);
            while (_engine != null && _engine.IsRunning && count <= maxCount)
            {
                Thread.Sleep(2000);
                count++;
                CommonFunctions.Log(string.Empty, "Process running.  Waiting....", null, LogLevel.Info);
            }

            if (count > maxCount)
            {
                msg = "Shutdown did not happen in a timely fashion.  Killing service forcefully.";
                eventLogger.WriteEntry(msg);
                CommonFunctions.Log(string.Empty, msg, null, LogLevel.Info);
            }

            Cleanup();
            msg = "Service Stopped.";
            eventLogger.WriteEntry(msg);
            CommonFunctions.Log(string.Empty, msg, null, LogLevel.Info);
        }

        private void Cleanup()
        {
            if (_engine != null)
                _engine.Dispose();
        }
    }
}
