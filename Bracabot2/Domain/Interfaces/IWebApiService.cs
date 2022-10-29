using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracabot2.Domain.Interfaces
{
    public interface IWebApiService
    {
        Task<T> CallApiAsync<T>(string url, IDictionary<string, string> headers = null);
    }
}
