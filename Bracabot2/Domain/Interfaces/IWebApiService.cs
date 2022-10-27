using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracabot2.Domain.Interfaces
{
    public interface IWebApiService
    {
        Task<T> GetAsync<T>(string url, IDictionary<string, string> headers = null);
        Task<T> PostAsync<T>(string url, object body);
    }
}
