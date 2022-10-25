using Bracabot2.Domain.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracabot2.Domain.Interfaces
{
    public interface IDotaService
    {
        Task<DotaApiHeroResponse> GetHeroAsync(string dotaId, string idHero);
        Task<string> GetIdAsync(string heroName);
        Task<string> GetMedalAsync(int medalId);
        Task<DotaApiMmrBucketResponse> GetMmrBucketAsync();
        Task<string> GetNameAsync(string heroId);
        Task<DotaApiPlayerResponse> GetPlayerAsync(string dotaId);
        Task<IEnumerable<DotaApiRecentMatchResponse>> GetRecentMatchesAsync(string dotaId);
    }
}
