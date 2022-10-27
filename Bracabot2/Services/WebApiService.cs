using Bracabot2.Domain.Interfaces;
using System.Text.Json;

namespace Bracabot2.Services
{
    public class WebApiService : IWebApiService
    {
        private async Task<string> GetAsync(string url, IDictionary<string, string> headers = default)
        {
            using var cliente = new HttpClient();

            if (headers != default)
            {
                foreach(var header in headers)
                    cliente.DefaultRequestHeaders.Add(header.Key, header.Value);
            }            

            var resposta = await cliente.GetAsync(url);

            return resposta.IsSuccessStatusCode 
                ? await resposta.Content.ReadAsStringAsync()
                : default;
        }

        private async Task<string> PostAsync(string url)
        {
            using var cliente = new HttpClient();

            var bodyContent = new StringContent(JsonSerializer.Serialize(string.Empty));

            var resposta = await cliente.PostAsync(url, bodyContent);

            return resposta.IsSuccessStatusCode
                ? await resposta.Content.ReadAsStringAsync()
                : default;
        }

        public async Task<T> GetAsync<T>(string url, IDictionary<string, string> headers = default)
        {
            return JsonSerializer.Deserialize<T>(await GetAsync(url, headers));
        }

        public async Task<T> PostAsync<T>(string url, object body)
        {
            return JsonSerializer.Deserialize<T>(await PostAsync(url));
        }
    }
}
