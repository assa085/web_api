using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static UI.MainWindow;

namespace UI
{
    internal class SenderRequests
    {
        private HttpClient _httpClient;
        public SenderRequests(string uri) 
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(uri)
            };
        }
        public async Task<bool> GetStatus()
        {
            var data = new MessageData("/status");
            var jsons = await GetJsons(data);

            if (jsons == null || jsons.Count == 0)
            {
                return false;
            }

            if (jsons.Any(j => j.Status != "OK"))
            {
                return false;
            }
            return true;
        }
        public async Task<string> GetRequest(MessageData data)
        {
            try
            {
                var jsons = await GetJsons(data);
                if (jsons == null || !jsons.Any())
                {
                    return "Данные не найдены.";
                }
                string text = string.Join("\n", jsons.Select(j => j.ToString()));

                return $"Сервер ответил JSON-ом: \n{text}";
            }
            catch (Exception ex)
            {
                return $"Ошибка связи: {ex.Message}";
            }
        }
        async Task<List<JsonData>> GetJsons(MessageData data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/data", data);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                    var jsons = CraftObjkt(result);
                    return jsons;
                }
                throw new Exception(response.StatusCode.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        List<JsonData> CraftObjkt(System.Text.Json.JsonElement data)
        {
            var resultList = new List<JsonData>();
            if (data.ValueKind == JsonValueKind.Array && data.GetArrayLength() > 0)
            {
                foreach (var firstItem in data.EnumerateArray())
                {
                    resultList.Add(new JsonData(firstItem));
                }
                return resultList;
            }
            return new List<JsonData> { new JsonData { Error = "Пустые данные" } };
        }
    }
}
