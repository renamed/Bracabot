using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bracabot2.Services
{
    public abstract class WebApiServiceBase
    {
        protected async Task<string> GetApiResponse(string url, IDictionary<string, string> headers = default)
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

        protected async Task<T> CallApiAsync<T>(string url, IDictionary<string, string> headers = default)
        {
            return JsonSerializer.Deserialize<T>(await GetApiResponse(url, headers));
        }
    }
}
