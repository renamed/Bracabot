using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;

namespace Bracabot2.Services
{
    public class DotaService : IDotaService
    {
        private readonly IWebApiService webApiService;
        private readonly SettingsOptions options;

        public DotaService(IWebApiService webApiService, IOptions<SettingsOptions> options)
        {
            this.webApiService = webApiService;
            this.options = options.Value;
        }

        public async Task<DotaApiPlayerResponse> GetPlayerAsync(string dotaId)
        {
            return await CallDotaApiAsync<DotaApiPlayerResponse>($"/players/{dotaId}");
        }

        public async Task<IEnumerable<DotaApiRecentMatchResponse>> GetRecentMatchesAsync(string dotaId)
        {
            return await CallDotaApiAsync<IEnumerable<DotaApiRecentMatchResponse>>($"/players/{dotaId}/recentmatches");
        }

        public async Task<DotaApiHeroResponse> GetHeroAsync(string dotaId, string idHero)
        {
            var heroes = await CallDotaApiAsync<IEnumerable<DotaApiHeroResponse>>($"/players/{dotaId}/heroes");

            var hero = heroes.FirstOrDefault(h => h.HeroId == idHero);
            return hero?.HeroId == idHero.ToString() ? hero : default;
        }

        public async Task<DotaApiMmrBucketResponse> GetMmrBucketAsync()
        {
            return await CallDotaApiAsync<DotaApiMmrBucketResponse>("/distributions");
        }

        public Task<string> GetNameAsync(string heroId)
        {
            return Task.FromResult(options.HeroesFromId[heroId]);
        }

        public Task<string> GetIdAsync(string heroName)
        {
            return Task.FromResult(!options.HeroesFromName.TryGetValue(heroName, out var heroId)
                ? null
                : heroId.ToString());
        }

        public Task<string> GetMedalAsync(int medalId)
        {
            return Task.FromResult(!options.Medals.TryGetValue(medalId, out var medalName)
                ? null
                : medalName.ToString());
        }

        private async Task<T> CallDotaApiAsync<T>(string suffix)
        {
            return await webApiService.GetAsync<T>($"https://api.opendota.com/api{suffix}");
        }
    }
}
