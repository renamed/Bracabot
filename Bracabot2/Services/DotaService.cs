using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace Bracabot2.Services
{
    public class DotaService : IDotaService
    {
        private readonly SettingsOptions options;
        private readonly HttpClient httpClient;
        private readonly IMemoryCache cache;
        private readonly ILogger logger;

        public DotaService(IOptions<SettingsOptions> options, HttpClient httpClient, IMemoryCache cache)
        {
            this.options = options.Value;
            this.httpClient = httpClient;

            logger = Log.ForContext<DotaService>();
            this.cache = cache;
        }


        public async Task<DotaApiPlayerResponse> GetPlayerAsync(string dotaId)
        {
            return await cache.GetOrCreateAsync("DotaService.GetPlayerAsync", async e =>
            {
                var player = await CallDotaApiAsync<DotaApiPlayerResponse>(string.Format(options.Apis.Dota.Players, dotaId));
                if (player == null)
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                else
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                return player;
            });
        }

        public async Task<IEnumerable<DotaApiRecentMatchResponse>> GetRecentMatchesAsync(string dotaId)
        {
            return await cache.GetOrCreateAsync("DotaService.GetRecentMatchesAsync", async e =>
            {                
                var recentMatches = await CallDotaApiAsync<IEnumerable<DotaApiRecentMatchResponse>>(string.Format(options.Apis.Dota.RecentMatches, dotaId));
                if (recentMatches == null)
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                else
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return recentMatches;
            });
        }

        public async Task<DotaApiHeroResponse> GetHeroStatisticsForPlayerAsync(string dotaId, string idHero)
        {
            var heroes = await cache.GetOrCreateAsync($"DotaService.GetHeroStatisticsForPlayerAsync", async e =>
            {
                var heroes = await CallDotaApiAsync<IEnumerable<DotaApiHeroResponse>>(string.Format(options.Apis.Dota.HeroStatisticsForPlayer, dotaId));
                if (heroes == null)
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                else
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return heroes;

            });

            if (heroes == null)
                return default;

            var hero = heroes.FirstOrDefault(h => h.HeroId == idHero);
            var heroStatistics = hero?.HeroId == idHero.ToString() ? hero : default;

            return heroStatistics;
        }

        public async Task<DotaApiMmrBucketResponse> GetMmrBucketAsync()
        {
            return await cache.GetOrCreateAsync("DotaService.GetMmrBucketAsync", async e =>
            {
                var mmrBucket = await CallDotaApiAsync<DotaApiMmrBucketResponse>(options.Apis.Dota.MmrBuckets);
                if (mmrBucket == null)
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                else
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

                return mmrBucket;
            });
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
