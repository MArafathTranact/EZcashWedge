using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace EZCashWedge
{
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        // Thread signal.
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private ArrayList handlerList;
        private ArrayList socketList;
        private int _portNumber;
        private string _yardId;
        public AsynchronousSocketListener(int portNumber, string yardId)
        {
            _portNumber = portNumber;
            _yardId = yardId;
        }


        public static string GetAppSettingValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }


        public void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];


            handlerList = new ArrayList();
            socketList = new ArrayList();

            IPAddress ipAddress = IPAddress.Parse(GetAppSettingValue("Ip"));
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _portNumber);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                LogEvents($" Listener created for Port '{_portNumber}'");
                LogEvents($" Waiting for a connection at {_portNumber}...");
                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.

                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at AsynchronousSocketListener.StartListening at port {_portNumber} : ", ex);
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.
                allDone.Set();

                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.
                StateObject state = new StateObject
                {
                    workSocket = handler
                };

                IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
                LogEvents($" Connection in at port {_portNumber} on {IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString())} , Port {((IPEndPoint)handler.RemoteEndPoint).Port}");
                LogEvents($" Waiting for a connection at port {_portNumber} ...");
                SocketHandler socketHandler = new SocketHandler(handler, _portNumber, _yardId);
                socketHandler.ListenClient();

                handlerList.Add(socketHandler);
                socketList.Add(handler);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at AsynchronousSocketListener.AcceptCallback at port {_portNumber} : ", ex);
            }

        }

        public async Task StopListener()
        {
            try
            {
                foreach (var item in handlerList)
                {
                    try
                    {
                        SocketHandler handler = (SocketHandler)item;
                        handler.DisconnectHandler();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogExceptionWithNoLock($" Exception at AsynchronousSocketListener.StopListener at port {_portNumber} : ", ex);
                    }

                }

                handlerList.Clear();
                socketList.Clear();
                handlerList = null;
                socketList = null;

            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at AsynchronousSocketListener.StopServer at port {_portNumber} : ", ex);
            }
        }



        public void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = state.sb.ToString();
                    //if (content.IndexOf("!") > -1)
                    //{
                    // All the data has been read from the 
                    // client. Display it on the console.
                    LogEvents($" Received {content}  , bytes from client {bytesRead} at port {_portNumber} .");
                    // Echo the data back to the client.
                    Send(handler, content);
                    //}
                    //else
                    //{
                    //    // Not all data received. Get more.
                    //    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    //    new AsyncCallback(ReadCallback), state);
                    //}
                }
                if (!handler.IsConnected())
                {
                    LogEvents($" Client discsonnected at port {_portNumber} .");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at AsynchronousSocketListener.ReadCallback at port {_portNumber} : ", ex);
            }

        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at AsynchronousSocketListener.Send at port {_portNumber} : ", ex);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                LogEvents($" Sent {bytesSent} bytes to client at port {_portNumber} .");
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

                StateObject state = new StateObject
                {
                    workSocket = handler
                };
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);


            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at AsynchronousSocketListener.SendCallback at port {_portNumber} : ", ex);
            }
        }

        private void LogEvents(string input)
        {
            Logger.LogWithNoLock($"{input}");
        }
    }
}
