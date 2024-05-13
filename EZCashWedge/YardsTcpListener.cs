using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedge
{

    public class YardInformation
    {
        public int PortNumber { get; set; }
        public string YardId { get; set; }
    }
    public class YardsTcpListener
    {

        List<YardInformation> yardInformations = new List<YardInformation>();
        public YardsTcpListener()
        {

            var yardIdCollection = ConfigurationManager.GetSection("yardIdSection") as NameValueCollection;

            if (yardIdCollection != null && yardIdCollection.AllKeys.Length != 0)
            {
                for (int i = 0; i < yardIdCollection.AllKeys.Length; i++)
                {
                    try
                    {
                        yardInformations.Add(new YardInformation { PortNumber = int.Parse(yardIdCollection.GetKey(i)), YardId = yardIdCollection.GetValues(i).FirstOrDefault() });

                    }
                    catch (Exception ex)
                    {
                        Logger.LogExceptionWithNoLock($" Exception at Reading YardId/Port Section :", ex);
                    }

                }
            }
            else
            {
                Logger.LogWarningWithNoLock($" YardSection Port/YardId is not available in config file to create listener .");
                return;
            }

        }



        List<AsynchronousSocketListener> lstAsyncListener = new List<AsynchronousSocketListener>();
        public async void CreateListeners()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                foreach (var port in yardInformations)
                {
                    AsynchronousSocketListener socketListener = new AsynchronousSocketListener(port.PortNumber, port.YardId);
                    tasks.Add(CreateListenerThread(socketListener));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at YardsTcpListener.CreateListeners", ex);
            }


        }

        public async Task StopYardListeners()
        {
            foreach (var item in lstAsyncListener)
            {
                try
                {
                    if (item != null)
                        await item.StopListener();

                }
                catch (Exception ex)
                {
                    Logger.LogExceptionWithNoLock($" Exception at YardsTcpListener.StopYardListeners", ex);
                }
            }

            lstAsyncListener.Clear();
            yardInformations.Clear();

        }


        private async Task CreateListenerThread(AsynchronousSocketListener socketListener)
        {
            try
            {
                lstAsyncListener.Add(socketListener);
                await Task.Run(() => { socketListener.StartListening(); });
            }
            catch (Exception)
            {
            }

        }
    }
}
