using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace Bracabot2.Services
{
    public class DotaService : IDotaService
    {
        private readonly SettingsOptions options;
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        public DotaService(IOptions<SettingsOptions> options, HttpClient httpClient)
        {
            this.options = options.Value;
            this.httpClient = httpClient;

            logger = Log.ForContext<DotaService>();
        }

        public async Task<DotaApiPlayerResponse> GetPlayerAsync(string dotaId)
        {
            return await CallDotaApiAsync<DotaApiPlayerResponse>(string.Format(options.Apis.Dota.Players, dotaId));
        }

        public async Task<IEnumerable<DotaApiRecentMatchResponse>> GetRecentMatchesAsync(string dotaId)
        {
            return await CallDotaApiAsync<IEnumerable<DotaApiRecentMatchResponse>>(string.Format(options.Apis.Dota.RecentMatches, dotaId));
        }

        public async Task<DotaApiHeroResponse> GetHeroStatisticsForPlayerAsync(string dotaId, string idHero)
        {
            var heroes = await CallDotaApiAsync<IEnumerable<DotaApiHeroResponse>>(string.Format(options.Apis.Dota.HeroStatisticsForPlayer, dotaId));

            var hero = heroes.FirstOrDefault(h => h.HeroId == idHero);
            return hero?.HeroId == idHero.ToString() ? hero : default;
        }

        public async Task<DotaApiMmrBucketResponse> GetMmrBucketAsync()
        {
            return await CallDotaApiAsync<DotaApiMmrBucketResponse>(options.Apis.Dota.MmrBuckets);
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
            logger.Debug("Dota API base address {0} - suffix {1}", httpClient.BaseAddress, suffix);

            var response = await httpClient.GetAsync(suffix);
            var responseJson = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                logger.Error("Dota API response error: Status code {0} .. Message: {1}", response.StatusCode, responseJson);
                return default;
            }

            logger.Debug("Dota API response JSON {0}", responseJson);
            return JsonSerializer.Deserialize<T>(responseJson);
        }
    }
}
