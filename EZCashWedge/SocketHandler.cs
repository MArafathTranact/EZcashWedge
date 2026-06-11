using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

    public class Device
    {
        public string encryption_key { get; set; }
        public int dev_id { get; set; }
        public string yardid { get; set; }
    }

    public class SocketHandler
    {
        public Socket handler = null;
        StateObject state;
        TestAPI testAPI = new TestAPI();
        private static readonly Encoding encoding = Encoding.UTF8;
        private readonly string ezCashAPI = ServiceConfiguration.GetFileLocation("EZCashAPI");
        private readonly string eZCashAPIToken = ServiceConfiguration.GetDecryptedToken("EZCashAPIToken");
        private readonly string wedgeType = ServiceConfiguration.GetFileLocation("WedgeType");
        private int _portNumber = 0;
        private string _yardId;
        private string _type = string.Empty;
        private string _encryptionKey = string.Empty;
        private string _encodeyardId = string.Empty;
        public SocketHandler(Socket clientSocket, int portNumber, string yardId)
        {
            handler = clientSocket;
            state = new StateObject { workSocket = clientSocket };
            _portNumber = portNumber;
            _yardId = yardId;

            _type = wedgeType == "0" ? $" Yard : {_yardId} " : $" Device : {_yardId}";

            if (wedgeType == "1" || wedgeType == "2")
                GetDeviceInformation();
        }

        private void GetDeviceInformation()
        {
            try
            {
                var api = ezCashAPI.Replace("customer_barcodes", "devices");
                var result = testAPI.GetRequestNew<Device>($"/{_yardId}", api, eZCashAPIToken);

                LogEvents($" Fetching device information.");
                if (result != null)
                {

                    _encodeyardId = result.yardid;
                    LogEvents($" Device YardId '{_encodeyardId}'.");
                    if (!string.IsNullOrEmpty(result.encryption_key))
                    {
                        _encryptionKey = result.encryption_key;

                        LogEvents($" Encryption key '{_encryptionKey}'.");
                    }
                    else
                    {
                        Logger.LogWarningWithNoLock($" No device encryption key found.");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" {_type} Exception at GetDeviceInformation at Port {_portNumber} . : ", ex);
            }

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

            if (wedgeType == "0" || wedgeType == "2")
            {
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
            else
            {
                LogEvents($" Entering barcode decoding at Port {_portNumber} ..");
                response = await ProcessBarcodeDecodingCommand(command, request);
                SendNonWebResponse(handler, response, true);
            }
        }

        private async Task<string> ProcessBarcodeDecodingCommand(string rawInput, string request)
        {
            try
            {
                int start = rawInput.IndexOf('=');
                int end = rawInput.IndexOf('?');

                if (start == -1 || end == -1 || end <= start)
                {
                    Logger.LogWarningWithNoLock(" Invalid message format: Missing delimiters.");
                    return "FAIL";
                }

                string dataSegment = rawInput.Substring(start + 1, end - start - 1);

                // 2. Extract the last 16 digits (Encrypted16)
                if (dataSegment.Length < 16)
                {
                    Logger.LogWarningWithNoLock($" Data segment too short to contain Encrypted16 block. {dataSegment}");
                    return "FAIL";
                }

                // Step 1: Get the 16 digits before the '='
                string pan = rawInput.Split('=')[0].TrimStart(';');

                // Step 2: The 'Passed' value is the last 4 digits of that PAN
                string passedChecksum = pan.Substring(pan.Length - 4);

                // Step 3: The 'Calculated' value is the same extraction
                // If the string was cut off, pan.Length would be wrong and this would fail.
                string calculatedChecksum = pan.Substring(12, 4);

                if (passedChecksum != calculatedChecksum)
                {
                    Logger.LogWarningWithNoLock($" Passed checksum({passedChecksum}) doesn't match with calcualted checksum ({calculatedChecksum}).");
                    return "FAIL";
                }

                LogEvents($" Passed Checksum = {passedChecksum}");
                LogEvents($" Calculated Checksum = {calculatedChecksum}");

                string encrypted16 = dataSegment.Substring(dataSegment.Length - 16);

                LogEvents($" Encrypted 16 Digit : {encrypted16}");

                LogEvents($" Decrypt with key : {_encryptionKey}");

                // 3. Perform the Digit-by-Digit Subtraction
                string decrypted16 = DecryptNumericString(encrypted16, _encryptionKey);

                LogEvents($" Decrypted 16 Digit : {decrypted16}");


                string amountRaw = decrypted16.Substring(0, 6);
                decimal properAmount = ParseToDecimal(amountRaw);

                LogEvents($" Amount : {properAmount}");

                string receiptNumber = GetReceiptNumber(rawInput);
                LogEvents($" Receipt  : {receiptNumber}");
                string dateRaw = decrypted16.Substring(10, 6);
                LogEvents($" Date  : {dateRaw}");
                var ezcashRequest = new EzCashAPIRequest
                {
                    payment_nbr = receiptNumber,
                    amount = properAmount,
                    yard_id = _encodeyardId

                };

                if (string.IsNullOrWhiteSpace(ezcashRequest.payment_nbr) || ezcashRequest.payment_nbr.Contains("Error"))
                {
                    LogEvents($" No/Invalid Payment Number in command at Port {_portNumber} .");
                    return "FAIL";
                }

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
                        LogEvents($" Encode is Success at Port {_portNumber} ..");
                        LogEvents($" TranId:{ezCashresponse.TranID}  AmtAuth:{ezCashresponse.amount}  Barcode:{barcode} at Port {_portNumber} .");
                        if (ezCashresponse.CardStatus.ToLower() == "duplicate")
                        {
                            LogEvents($" Card already exists.");
                            LogEvents($" Sending DUPLICATE {barcode} for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                            return $"DUPLICATE {ezCashresponse.Barcode}";
                        }
                        else
                        {
                            LogEvents($" Sending SUCCESS {barcode} for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                            return $"SUCCESS {ezCashresponse.Barcode}";
                        }
                    }
                    else
                    {
                        LogEvents($" Encode failed for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
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
                Logger.LogExceptionWithNoLock(" {_type} Exception at ProcessBarcodeDecodingCommand at Port {_portNumber} . : ", ex);
                return "FAIL";
            }


        }

        public string GetReceiptNumber(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
                return "Error: Input is empty";

            var match = Regex.Match(rawInput, @"^;(\d{6})(\d{6})(\d{4})=");

            if (match.Success)
            {
                // Group 0 is the full match, Group 1 is first 6, Group 2 is receipt number.
                string ConistenChecksum = match.Groups[1].Value;
                string receiptNumber = match.Groups[2].Value;
                string checksum = match.Groups[3].Value;

                return receiptNumber;
            }
            else
            {
                return "Error: Message does not follow the 6-6-4 digit validation rules.";
            }
        }

        private decimal ParseToDecimal(string raw)
        {
            if (!long.TryParse(raw, out long kopeks))
                return 0.00m;

            // Dividing by 100 converts the "cents" into the proper decimal place
            return kopeks / 100m;
        }

        private string DecryptNumericString(string ciphertext, string key)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < ciphertext.Length; i++)
            {
                // Convert char digit to int (e.g., '2' becomes 2)
                int cDigit = ciphertext[i] - '0';
                int kDigit = key[i] - '0';

                // Modular subtraction: (C - K + 10) % 10
                int rDigit = (cDigit - kDigit + 10) % 10;

                sb.Append(rDigit);
            }

            return sb.ToString();
        }


        public async Task<T> Get<T>(string path, string token, string endpoint, string command, string paymentNumber)
        {
            var httpResponseString = string.Empty;

            try
            {
                //IsAPISuccess = true;
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
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
                Logger.LogExceptionWithNoLock($" {_type} Exception at Get() at Port {_portNumber} .: ", ex);
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

                dynamic input = new ExpandoObject();

                foreach (var item in splittedRequest)
                {
                    var split = item.Split(new string[] { "=<" }, StringSplitOptions.None);
                    var filter = split[0].Trim().ToLower();
                    switch (filter)
                    {
                        case var s when filter.Contains("payment_nbr"):
                            ezcashRequest.payment_nbr = split[1];
                            input.payment_nbr = split[1];
                            break;
                        case var s when filter.Contains("amount"):
                            ezcashRequest.amount = decimal.Parse(split[1]);
                            input.amount = split[1];
                            break;
                        //case var s when filter.Contains("date"): // To avoid timestamp issue in UI, removed passing date on encode call
                        //    ezcashRequest.date = split[1];
                        //    break;
                        case var s when filter.Contains("cashier_id"):
                            ezcashRequest.cashier_id = split[1];
                            input.cashier_id = split[1];
                            break;
                        case var s when filter.Contains("device_id"):
                            ezcashRequest.device_id = split[1];
                            input.device_id = split[1];
                            break;
                        case var s when filter.Contains("payee"):
                            ezcashRequest.payee = split[1];
                            input.payee = split[1];
                            break;
                    }
                }

                if (wedgeType == "2")
                {
                    ezcashRequest.yard_id = _encodeyardId;
                    ezcashRequest.device_id = _yardId; // In wedgetype case=2 , yardid is device id

                    input.device_id = _yardId;
                    input.yard_id = _encodeyardId;
                }
                else
                {
                    ezcashRequest.yard_id = _yardId;
                    input.yard_id = _yardId;
                }

                if (string.IsNullOrWhiteSpace(ezcashRequest.payment_nbr))
                {
                    LogEvents($" No Payment Number in command at Port {_portNumber} .");
                    return "FAIL";
                }


                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };

                if (!string.IsNullOrWhiteSpace(eZCashAPIToken))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", eZCashAPIToken);

                var endpoint = ezCashAPI + "encode";

                var json = JsonConvert.SerializeObject(input);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                LogEvents($" Creating barcode using endpoint :{endpoint}, payload ={json} at port {_portNumber}.");

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
                        LogEvents($" Encode is Success at Port {_portNumber} ..");
                        LogEvents($" TranId:{ezCashresponse.TranID}  AmtAuth:{ezCashresponse.amount}  Barcode:{barcode} at Port {_portNumber} .");
                        if (ezCashresponse.CardStatus.ToLower() == "duplicate")
                        {
                            LogEvents($" Card already exists. \r\n{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}|INFO| Sending DUPLICATE {barcode} for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                            return $"DUPLICATE {ezCashresponse.Barcode}";
                        }
                        else
                        {
                            LogEvents($" Sending SUCCESS {barcode} for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                            return $"SUCCESS {ezCashresponse.Barcode}";
                        }
                    }
                    else
                    {
                        LogEvents($" Encode failed for Payment Number '{ezcashRequest.payment_nbr}' from Port {_portNumber}");
                        return "FAIL";
                    }
                }
                else
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    Logger.LogWarningWithNoLock($" {_type} Encode failed for Receipt number {ezcashRequest.payment_nbr} : Failure Code {responseBody} at Port {_portNumber} .Sending FAIL status.");
                    return "FAIL";
                }

            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock(" {_type} Exception at ProcessEncodeCommand at Port {_portNumber} . : ", ex);
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
                    LogEvents($" No Payment Number in command at Port {_portNumber} .");

                var voidParams = $"void?payment_nbr={ezcashRequest.payment_nbr}&date={string.Format("{0:yyyy-MM-ddTHH:mm}", ezcashRequest.date)}&amount={ezcashRequest.amount}&yard_id={ezcashRequest.yard_id}";

                var ezcashResponse = await Get<EzCashResponse>(voidParams, eZCashAPIToken, ezCashAPI, "Void", ezcashRequest.payment_nbr);

                if (ezcashResponse != null && ezcashResponse.CardStatus.ToLower().Contains("partial"))
                {
                    LogEvents($" Success Void API call for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber} .");
                    status = ezcashResponse.CardStatus + $" {ezcashResponse.PartialPayPaidAmount}" + $" of {ezcashResponse.PartialPayTotal}";
                }
                else if (ezcashResponse != null && ezcashResponse.CardStatus.ToLower().Contains("voided"))
                {
                    LogEvents($" Success Void API call for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber}. Sending {ezcashResponse.CardStatus}.");
                    status = "SUCCESS";
                }
                else if (ezcashResponse != null)
                {
                    LogEvents($" Success Void API call for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber}. Error= {ezcashResponse.error},  Sending {ezcashResponse.CardStatus}.");
                    status = ezcashResponse.CardStatus;
                }
                else
                {
                    LogEvents($" Void is Failed for Payment Number '{ezcashRequest.payment_nbr}' at Port {_portNumber}. Sending FAILED.");
                    status = "FAILED";
                }

            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock($" {_type} Exception at ProcessVoidCommand at Port {_portNumber}. Sending FAILED : ", ex);
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
                Logger.LogExceptionWithNoLock($" {_type} Exception at ProcessInquireCommand at Port {_portNumber}. Sending {status}: ", ex);
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
                Logger.LogExceptionWithNoLock($" {_type} Exception at SocketHandler.SendCallback at Port {_portNumber} .:", ex);
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
            Logger.LogWithNoLock($"{_type}{input}");
        }
    }

    public class TestAPI
    {

        public bool GetRequest(string param, string endPoint, string token)
        {
            string responseBody = string.Empty;
            var method = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
                    //client.Timeout = TimeSpan.FromSeconds(APITimeOut);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    method = endPoint + param;
                    using (HttpResponseMessage response = client.GetAsync(method).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            responseBody = response.Content.ReadAsStringAsync().Result;
                            return true;

                        }
                        else
                        {
                            //MessageBox.Show($"Failure Code : {response.ReasonPhrase}", "Failure");
                            return false;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;

            }


        }

        public T GetRequestNew<T>(string param, string endPoint, string token)
        {
            string responseBody = string.Empty;
            var method = "";


            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
                    //client.Timeout = TimeSpan.FromSeconds(APITimeOut);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    method = endPoint + param;
                    using (HttpResponseMessage response = client.GetAsync(method).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            responseBody = response.Content.ReadAsStringAsync().Result;

                            var result = JsonConvert.DeserializeObject<T>(responseBody);

                            return result;

                        }
                        else
                        {
                            //MessageBox.Show($"Failure Code : {response.ReasonPhrase}", "Failure");
                            return default;
                        }
                    }
                }
            }
            catch (Exception)
            {

                return default;
            }
        }
    }
}
