using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LMStudio.Acceso_a_datos
{
    public class LMApi
    {
        private static readonly object _lock = new object();
        private static string baseUrl = "http://localhost:1234/v1";
        private static RestClient _restClientInstance;
        private static RestClient RestClientInstance
        {
            get
            {
                if (_restClientInstance == null)
                {
                    lock (_lock)
                    {
                        if (_restClientInstance == null)
                        {
                            _restClientInstance = new RestClient(baseUrl);
                        }
                    }
                }
                return _restClientInstance;
            }
        }

        public static async Task<string> PostAPIAsync(string message, string model)
        {
            try
            {
                var request = new RestRequest("/chat/completions", Method.Post);
                request.AddJsonBody(new
                {
                    model = model,
                    messages = new List<object>
                    {
                        new { role = "system", content = "Siempre responder en rimas" },
                        new { role = "user", content = message }
                    },
                            temperature = 0.5
                        });

                var response = await RestClientInstance.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    dynamic data = JsonConvert.DeserializeObject(response.Content);
                    string result = data.choices[0].message.content;
                    return result;
                }
                else
                {
                    throw new Exception("Error al hacer la solicitud: " + response.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al hacer la solicitud: " + e.Message);
            }
        }
    }
}
