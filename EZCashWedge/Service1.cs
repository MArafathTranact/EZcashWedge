using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace EZCashWedge
{
    public partial class Service1 : ServiceBase
    {
        //        AsynchronousSocketListener socketListener;
        YardsTcpListener yardsTcpListener;

        private CancellationTokenSource _serviceCts;
        private Task _socketTask;
        private Task _updateTask;
        
        private readonly string _updateOnServiceStart = ServiceConfiguration.GetFileLocation("UpdateOnServiceStart");


        public Service1()
        {
            InitializeComponent();          
            //ConnectSocketListener();          
        }

        protected override void OnStart(string[] args)
        {
            Logger.LogWithNoLock($" Service Started ");
            Logger.LogWithNoLock($" Version Number : 1.1.1");
            Logger.LogWithNoLock($" -------- Maximum file size for the log is 100 MB --------");
            _serviceCts = new CancellationTokenSource();

            try
            {

                if (!string.IsNullOrEmpty(_updateOnServiceStart) && _updateOnServiceStart == "1")
                    _updateTask = Task.Run(() => CheckUpdateAsync(_serviceCts.Token));
                else
                    LogEvents(" No service auto update configured.");

                _socketTask = Task.Run(() => ConnectSocketListener());
            }
            catch (Exception)
            {

            }
        }

        protected override void OnStop()
        {
            try
            {
                Logger.LogWithNoLock($" Stoping Service..");

                // 2. Clean temporary files safely
                string tempFolder = Path.Combine(Path.GetTempPath(), "EZCashWedgeAutoUpdate");
                if (Directory.Exists(tempFolder))
                {
                    try
                    {
                        LogEvents(" Deleting previous temp EZCashWedgeAutoUpdate folder");
                        Directory.Delete(tempFolder, true);
                    }
                    catch (Exception ex)
                    {
                        LogException(" Non-critical failure cleaning update directories.", ex);
                    }
                }

                yardsTcpListener.StopYardListeners();

                // 4. Block synchronously for up to 3 seconds to let active threads unpack and exit cleanly
                Task.WaitAll(new[] { _socketTask, _updateTask }.WaitReadyTasks(), 3000);

                LogEvents(" Service successfully stopped and resources freed.");

                Logger.LogWithNoLock($" Service stopped ");
                Task.Delay(1000);


            }
            catch (Exception)
            {
                Logger.LogWithNoLock($" Service stopped ");
                Task.Delay(1000);
            }
        }

        private async Task ConnectSocketListener()
        {
            try
            {
                yardsTcpListener = new YardsTcpListener();
                yardsTcpListener.CreateListeners();
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at ConnectSocket : ", ex);
            }
        }

        private async Task CheckUpdateAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Safe non-blocking startup buffer
                await Task.Delay(2000, cancellationToken).ConfigureAwait(false);

                string tempFolder = Path.Combine(AppContext.BaseDirectory, "serviceupdate.txt");
                if (!File.Exists(tempFolder))
                {
                    LogEvents(" Checking for update.");

#if DEBUG
                    var executablePath = @"C:\Program Files (x86)\EZCash\EZCashWedge\EZCashWedgeLazyAutoUpdate.exe";
#else
                    string servicePath = AppContext.BaseDirectory;
                    var executablePath = Path.Combine(servicePath, "EZCashWedgeLazyAutoUpdate.exe");
#endif

                    if (File.Exists(executablePath))
                    {
                        LogEvents(" Launching auto updater.");
                        ApplicationLoader.PROCESS_INFORMATION procInfo;
                        ApplicationLoader.StartProcessAndBypassUAC(executablePath, "", out procInfo);
                    }
                    else
                    {
                        LogEvents($" Update executable not found at: {executablePath}");
                    }
                }
                else
                {
                    LogEvents(" Update in progress. Skipping Autoupdater initialization on OnStart().");
                }
            }
            catch (OperationCanceledException ox)
            {
                LogException(" CheckUpdate operation was cancelled during service shutdown.", ox);
            }
            catch (Exception ex)
            {
                LogException(" Exception encountered during background update check execution.", ex);
            }
        }

        private void LogEvents(string input)
        {
            Logger.LogWithNoLock(input);
        }

        private void LogException(string input, Exception ex)
        {
            Logger.LogExceptionWithNoLock(input, ex);
        }

    }

    public static class TaskExtensions
    {
        // Helper to filter out unallocated tasks before calling Task.WaitAll
        public static Task[] WaitReadyTasks(this Task[] tasks)
        {
            return Array.FindAll(tasks, t => t != null);
        }
    }
}
