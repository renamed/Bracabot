using Bracabot2.Domain.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bracabot2.Services
{
    public class DotaService : WebApiServiceBase
    {
        private static Dictionary<string, string> idToNameHeroes;
        private static Dictionary<string, int> nameToIdHeroes;
        private static Dictionary<int, string> idMedals;

        private static async Task InitializeConsts()
        {
            if (idToNameHeroes is null || nameToIdHeroes is null || idMedals is null)
            {
                idToNameHeroes = new Dictionary<string, string>(JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync("hero_from_id.json")), StringComparer.CurrentCultureIgnoreCase);
                nameToIdHeroes = new Dictionary<string, int>(JsonSerializer.Deserialize<Dictionary<string, int>>(await File.ReadAllTextAsync("hero_from_name.json")), StringComparer.CurrentCultureIgnoreCase);
                idMedals = JsonSerializer.Deserialize<Dictionary<int, string>>(await File.ReadAllTextAsync("id_medals.json"));
            }
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

        public async Task<string> GetNameAsync(string heroId)
        {
            await InitializeConsts();
            return idToNameHeroes[heroId];
        }

        public async Task<string> GetIdAsync(string heroName)
        {
            await InitializeConsts();

            return !nameToIdHeroes.TryGetValue(heroName, out var heroId)
                ? null
                : heroId.ToString();
        }

        public async Task<string> GetMedalAsync(int medalId)
        {
            await InitializeConsts();

            return !idMedals.TryGetValue(medalId, out var medalName)
                ? null
                : medalName.ToString();
        }

        private async Task<T> CallDotaApiAsync<T>(string suffix)
        {
            return await CallApiAsync<T>($"https://api.opendota.com/api{suffix}");
        }
    }
}
