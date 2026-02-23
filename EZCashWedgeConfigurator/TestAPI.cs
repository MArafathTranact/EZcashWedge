using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedgeConfigurator
{
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
                            MessageBox.Show($"Failure Code : {response.ReasonPhrase}", "Failure");
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
                            MessageBox.Show($"Failure Code : {response.ReasonPhrase}", "Failure");
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
