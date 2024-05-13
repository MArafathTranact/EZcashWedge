using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace EZCashWedge
{
    static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }

    public class SocketHandler
    {
        public Socket handler = null;
        StateObject state;
        private static readonly Encoding encoding = Encoding.UTF8;
        private readonly string ezCashAPI = ConfigurationManager.AppSettings["EZCashAPI"];// GetFileLocation("EZCashAPI");
        private readonly string eZCashAPIToken = ConfigurationManager.AppSettings["EZCashAPIToken"];  //GetFileLocation("EZCashAPIToken");
        private int _portNumber = 0;
        private string _yardId;
        public SocketHandler(Socket clientSocket, int portNumber, string yardId)
        {
            handler = clientSocket;
            state = new StateObject { workSocket = clientSocket };
            _portNumber = portNumber;
            _yardId = yardId;
        }

        public string GetFileLocation(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public void ListenClient()
        {
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                 new AsyncCallback(ReadCallback), state);
        }

        public async void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String request = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    request = state.sb.ToString();

                    LogEvents($" Received {bytesRead} bytes at Port {_portNumber} .");

                    LogEvents($" Processing EZcash request at Port {_portNumber} .");
                    LogEvents($" Processing  : {request} at Port {_portNumber} .");
                    var command = request.Split(' ')[0].Trim().ToLower();
                    await ParseEZCashRequest(request, command);

                }
                if (handler.Connected && !handler.IsConnected())
                {
                    LogEvents($" Client disconnected at Port {_portNumber} .");
                    handler.Shutdown(SocketShutdown.Both);

                }
            }
            catch (Exception ex)
            {
                // LogEvents($" Client disconnected at Port {_portNumber} .");
                // Logger.LogExceptionWithNoLock($" Exception at SocketHandler.ReadCallback at Port {_portNumber} .:", ex);
            }

        }


        private async Task ParseEZCashRequest(string request, string command)
        {
            var response = string.Empty;

            switch (command)
            {
                case "encode":
                    LogEvents($" Entering encode at Port {_portNumber} ..");
                    response = await ProcessEncodeCommand(command, request);
                    SendNonWebResponse(handler, response, true);
                    break;
                case "void":
                    LogEvents($" Entering void at Port {_portNumber} ..");
                    response = await ProcessVoidCommand(command, request);
                    SendNonWebResponse(handler, response, true);
                    break;
                case "inquire":
                    LogEvents($" Entering inquire at Port {_portNumber} ..");
                    response = await ProcessInquireCommand(command, request);
                    SendNonWebResponse(handler, response, false);
                    break;
            }

        }

        public async Task<T> Get<T>(string path, string token, string endpoint, string command, string paymentNumber)
        {
            var httpResponseString = string.Empty;

            try
            {
                //IsAPISuccess = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };

                if (!string.IsNullOrWhiteSpace(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);

                var httpResponse = await httpClient.GetAsync(endpoint + path);
                if (httpResponse.IsSuccessStatusCode)
                {
                    httpResponseString = await httpResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    var responseBody = httpResponse.Content.ReadAsStringAsync().Result;
                    Logger.LogWarningWithNoLock($" {command} failed for Receipt number {paymentNumber} : Failure Code '{responseBody}' at Port {_portNumber} .");
                }
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at Get() at Port {_portNumber} .: ", ex);
                return default;
            }
            return JsonConvert.DeserializeObject<T>(httpResponseString);
        }

        private async Task<string> ProcessEncodeCommand(string command, string request)
        {
            try
            {
                var splittedRequest = request.Replace(command, "").Split('>');
                var ezcashRequest = new EzCashAPIRequest();
                foreach (var item in splittedRequest)
                {
                    var split = item.Split(new string[] { "=<" }, StringSplitOptions.None);
                    var filter = split[0].Trim().ToLower();
                    switch (filter)
                    {
                        case var s when filter.Contains("payment_nbr"):
                            ezcashRequest.payment_nbr = split[1];
                            break;
                        case var s when filter.Contains("amount"):
                            ezcashRequest.amount = decimal.Parse(split[1]);
                            break;
                        case var s when filter.Contains("date"):
                            ezcashRequest.date = split[1];
                            break;
                    }
                }
                ezcashRequest.yard_id = _yardId;

                if (string.IsNullOrWhiteSpace(ezcashRequest.payment_nbr))
                    Logger.LogWithNoLock(" No Payment Number in command at Port {_portNumber} .");


                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };

                if (!string.IsNullOrWhiteSpace(eZCashAPIToken))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", eZCashAPIToken);

                var endpoint = ezCashAPI + "encode";

                var json = JsonConvert.SerializeObject(ezcashRequest);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(endpoint, stringContent);
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var ezCashresponse = new EzCashResponse();
                    ezCashresponse = JsonConvert.DeserializeObject<EzCashResponse>(content);

                    if (ezCashresponse != null)
                    {
                        var barcode = ezCashresponse.Barcode.Substring(0, 5) + "...";
                        Logger.LogWithNoLock($" Encode is Success at Port {_portNumber} ..");
                        Logger.LogWithNoLock($" TranId:{ezCashresponse.TranID}  AmtAuth:{ezCashresponse.amount}  Barcode:{barcode} at Port {_portNumber} .");
                        if (ezCashresponse.CardStatus.ToLower() == "duplicate")
                        {
                            Logger.LogWithNoLock($" Card already exists. \r\n{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}|INFO| Sending DUPLICATE {barcode} for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                            return $"DUPLICATE {ezCashresponse.Barcode}";
                        }
                        else
                        {
                            Logger.LogWithNoLock($" Sending SUCCESS {barcode} for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                            return $"SUCCESS {ezCashresponse.Barcode}";
                        }
                    }
                    else
                    {
                        Logger.LogWithNoLock($" Encode failed for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                        return "FAIL";
                    }
                }
                else
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    Logger.LogWarningWithNoLock($" Encode failed for Receipt number {ezcashRequest.payment_nbr} : Failure Code {responseBody} at Port {_portNumber} .Sending FAIL status.");
                    return "FAIL";
                }

            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock("Exception at ProcessEncodeCommand at Port {_portNumber} . : ", ex);
                return "FAIL";
            }


        }

        private async Task<string> ProcessVoidCommand(string command, string request)
        {
            var status = "FAILED";

            try
            {
                var splittedRequest = request.Replace(command, "").Split('>');
                var ezcashRequest = new EzCashAPIRequest();
                foreach (var item in splittedRequest)
                {
                    var split = item.Split(new string[] { "=<" }, StringSplitOptions.None);
                    var filter = split[0].Trim().ToLower();
                    switch (filter)
                    {
                        case var s when filter.Contains("payment_nbr"):
                            ezcashRequest.payment_nbr = split[1];
                            break;
                        case var s when filter.Contains("amount"):
                            ezcashRequest.amount = decimal.Parse(split[1]);
                            break;
                        case var s when filter.Contains("date"):
                            ezcashRequest.date = split[1];
                            break;
                    }

                }

                ezcashRequest.yard_id = _yardId;

                if (string.IsNullOrWhiteSpace(ezcashRequest.payment_nbr))
                    Logger.LogWithNoLock($" No Payment Number in command at Port {_portNumber} .");

                var voidParams = $"void?payment_nbr={ezcashRequest.payment_nbr}&date={string.Format("{0:yyyy-MM-ddTHH:mm}", ezcashRequest.date)}&amount={ezcashRequest.amount}&yard_id={ezcashRequest.yard_id}";

                var ezcashResponse = await Get<EzCashResponse>(voidParams, eZCashAPIToken, ezCashAPI, "Void", ezcashRequest.payment_nbr);

                if (ezcashResponse != null && ezcashResponse.CardStatus.ToLower().Contains("partial"))
                {
                    Logger.LogWithNoLock($" Void is Success for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber} .");
                    status = ezcashResponse.CardStatus + $" {ezcashResponse.PartialPayPaidAmount}" + $" of {ezcashResponse.PartialPayTotal}";
                }
                else if (ezcashResponse != null && ezcashResponse.CardStatus.ToLower().Contains("voided"))
                {
                    Logger.LogWithNoLock($" Void is Success for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber}. Sending {ezcashResponse.CardStatus}.");
                    status = "SUCCESS";
                }
                else if (ezcashResponse != null)
                {
                    Logger.LogWithNoLock($" Void is Success for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber}. Sending {ezcashResponse.CardStatus}.");
                    status = ezcashResponse.CardStatus;
                }
                else
                {
                    Logger.LogWithNoLock($" Void is Failed for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber}. Sending FAILED.");
                    status = "FAILED";
                }

            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at ProcessVoidCommand at Port {_portNumber}. Sending FAILED : ", ex);
                return status;
            }

            return status;
        }

        private async Task<string> ProcessInquireCommand(string command, string request)
        {
            var status = "FAILED";
            try
            {
                var splittedRequest = request.Replace(command, "").Split('>');
                var ezcashRequest = new EzCashAPIRequest();
                foreach (var item in splittedRequest)
                {
                    var split = item.Split(new string[] { "=<" }, StringSplitOptions.None);
                    var filter = split[0].Trim().ToLower();
                    switch (filter)
                    {
                        case var s when filter.Contains("payment_nbr"):
                            ezcashRequest.payment_nbr = split[1];
                            break;
                        case var s when filter.Contains("amount"):
                            ezcashRequest.amount = decimal.Parse(split[1]);
                            break;
                        case var s when filter.Contains("date"):
                            ezcashRequest.date = split[1];
                            break;
                    }
                }

                ezcashRequest.yard_id = _yardId;

                if (string.IsNullOrWhiteSpace(ezcashRequest.payment_nbr))
                    LogEvents($" No Payment Number in command at Port {_portNumber} .");

                var inquireParams = $"inquire?payment_nbr={ezcashRequest.payment_nbr}&date={string.Format("{0:yyyy-MM-ddTHH:mm}", ezcashRequest.date)}&amount={ezcashRequest.amount}&yard_id={ezcashRequest.yard_id}";

                var ezcashResponse = await Get<EzCashResponse>(inquireParams, eZCashAPIToken, ezCashAPI, "Inquire", ezcashRequest.payment_nbr);

                if (ezcashResponse != null && !string.IsNullOrWhiteSpace(ezcashResponse.PaymentNumber))
                {
                    switch (ezcashResponse.CardStatus.ToLower())
                    {
                        case "unused":
                        case "active":
                            ezcashResponse.CardStatus = "AC";
                            break;
                        case "used":
                            ezcashResponse.CardStatus = "CL";
                            break;
                        case "voided":
                            ezcashResponse.CardStatus = "VD";
                            break;
                    }

                    if (ezcashResponse.CardStatus == "")
                        ezcashResponse.CardStatus = "AC";
                    if (ezcashResponse.CardStatus == "")
                        ezcashResponse.CardStatus = "AC";
                    if (ezcashResponse.CardStatus == "")
                        ezcashResponse.CardStatus = "AC";


                    status = $"payment_nbr=<{ezcashResponse.PaymentNumber}>barcode=<{ezcashResponse.Barcode}>initial_amt=<{ezcashResponse.InitialAmount}>avail_amt=<{ezcashResponse.AvailableAmount}>card_status=<{ezcashResponse.CardStatus}>!";
                }
                else if (ezcashResponse != null)
                {
                    status = ezcashResponse.CardStatus;
                }
                else
                {
                    Logger.LogWithNoLock($" Inquire is Failed for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber}. Sending {status}.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" Exception at ProcessInquireCommand at Port {_portNumber}. Sending {status}: ", ex);
                return status;
            }

            return status;
        }

        private void SendNonWebResponse(Socket handler, String data, bool needCL)
        {
            var hexstring = string.Empty;
            byte[] tempByte = null;
            if (needCL)
            {
                byte[] ba = Encoding.Default.GetBytes(data);
                hexstring = BitConverter.ToString(ba);
                hexstring = hexstring.Replace("-", "");
                hexstring += "0d0a";
                tempByte = StringToByteArray(hexstring, true);
            }


            //handler.Send(Encoding.UTF8.GetBytes(data));
            byte[] byteData = needCL == true ? tempByte : Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                   new AsyncCallback(SendCallback), handler);
        }

        private byte[] StringToByteArray(String hex, bool checkOdd)
        {
            int NumberChars = hex.Length;
            if (checkOdd && NumberChars % 2 != 0)
                NumberChars++;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                if (bytesSent == 0)
                    LogEvents($" No result to send at Port {_portNumber} ..");
                else
                    LogEvents($" Sent {bytesSent} bytes to client at Port {_portNumber} .");
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
                Logger.LogExceptionWithNoLock($" Exception at SocketHandler.SendCallback at Port {_portNumber} .:", ex);
            }
        }


        public void DisconnectHandler()
        {
            try
            {
                LogEvents($" Disconnecting client at Port {_portNumber} .");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception)
            {

            }

        }

        private void LogEvents(string input)
        {
            Logger.LogWithNoLock($"{input}");
        }
    }
}
