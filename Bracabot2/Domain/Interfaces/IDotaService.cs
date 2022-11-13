using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracabot2.Domain.Interfaces
{
    public interface IDotaService
    {
        Task<DotaApiHeroResponse> GetHeroStatisticsForPlayerAsync(string dotaId, string idHero);
        Task<string> GetIdAsync(string heroName);
        Task<IEnumerable<DotaApiMatchResponse>> GetMatchesAsync(string dotaId, int days);
        Task<string> GetMedalAsync(int medalId);
        Task<DotaApiMmrBucketResponse> GetMmrBucketAsync();
        Task<string> GetNameAsync(string heroId);
        Task<DotaApiPeersResponse> GetPeersAsync(string dotaId, string accountId);
        Task<DotaApiPlayerResponse> GetPlayerAsync(string dotaId);
    }
}
