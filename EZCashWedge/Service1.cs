using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedge
{
    public partial class Service1 : ServiceBase
    {
        //        AsynchronousSocketListener socketListener;
        YardsTcpListener yardsTcpListener;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.LogWithNoLock($" Service Started ");
            Logger.LogWithNoLock($" -------- Maximum file size for the log is 100 MB --------");
            try
            {
                Task.Factory.StartNew(() =>
                {
                    ConnectSocketListener();
                });
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

                yardsTcpListener.StopYardListeners();
                Logger.LogWithNoLock($" Service stopped ");
                Task.Delay(1000);
                NLog.LogManager.Shutdown();


            }
            catch (Exception)
            {
                Logger.LogWithNoLock($" Service stopped ");
                Task.Delay(1000);
                NLog.LogManager.Shutdown();
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
    }
}
