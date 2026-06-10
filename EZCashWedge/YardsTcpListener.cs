using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EZCashWedge
{

    public class YardInformation
    {
        public int PortNumber { get; set; }
        public string YardId { get; set; }
    }

    public class DeviceInformation
    {
        public int PortNumber { get; set; }
        public string DeviceId { get; set; }


    }
    public class YardsTcpListener
    {

        List<YardInformation> yardInformations = new List<YardInformation>();
        List<DeviceInformation> deviceInformations = new List<DeviceInformation>();
        List<AsynchronousSocketListener> lstAsyncListener = new List<AsynchronousSocketListener>();
        public YardsTcpListener()
        {

            var wedgeType = ServiceConfiguration.GetFileLocation("WedgeType");


            if (wedgeType == "0")
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
            else
            {
                var deviceCollection = ConfigurationManager.GetSection("deviceSection") as NameValueCollection;

                if (deviceCollection != null && deviceCollection.AllKeys.Length != 0)
                {
                    for (int i = 0; i < deviceCollection.AllKeys.Length; i++)
                    {
                        try
                        {
                            deviceInformations.Add(new DeviceInformation { PortNumber = int.Parse(deviceCollection.GetKey(i)), DeviceId = deviceCollection.GetValues(i).FirstOrDefault() });

                        }
                        catch (Exception ex)
                        {
                            Logger.LogExceptionWithNoLock($" Exception at Reading Device/Port Section :", ex);
                        }

                    }
                }
                else
                {
                    Logger.LogWarningWithNoLock($" DeviceSection Port/DeviceId is not available in config file to create listener .");
                    return;
                }
            }

        }


        public async void CreateListeners()
        {
            try
            {
                Thread.Sleep(10000);
                List<Task> tasks = new List<Task>();
                var wedgeType = ServiceConfiguration.GetFileLocation("WedgeType");

                if (wedgeType == "0")
                {
                    foreach (var port in yardInformations)
                    {
                        AsynchronousSocketListener socketListener = new AsynchronousSocketListener(port.PortNumber, port.YardId);
                        tasks.Add(CreateListenerThread(socketListener));
                    }
                }
                else
                {
                    foreach (var port in deviceInformations)
                    {
                        AsynchronousSocketListener socketListener = new AsynchronousSocketListener(port.PortNumber, port.DeviceId);
                        tasks.Add(CreateListenerThread(socketListener));
                    }
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
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at YardsTcpListener.CreateListenerThread", ex);
            }
        }
    }
}
