using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace Bracabot2.Services
{
    public class TwitchService : ITwitchService
    {
        private readonly SettingsOptions options;
        private readonly ILogger logger;
        private readonly HttpClient twitchApi;
        private readonly HttpClient tokenTwitchApi;
        private IMemoryCache cache;


        public TwitchService(IOptions<SettingsOptions> options, IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            this.options = options.Value;
            twitchApi = clientFactory.CreateClient(Consts.Clients.TWITCH_API_CLIENT);
            tokenTwitchApi = clientFactory.CreateClient(Consts.Clients.TWITCH_TOKEN_API_CLIENT);
            this.cache = cache;

            logger = Log.ForContext<TwitchService>();
            
        }

        public async Task<TwitchApiStreamInfoNodeResponse> GetStreamInfo()
        {
            return await cache.GetOrCreateAsync("TwitchService.GetStreamInfo", async e =>
            {
                var twitchBroadcastId = options.TwitchBroadcastId;
                var suffixUrl = string.Format(options.Apis.Twitch.StreamInfo, twitchBroadcastId);
                var info = await CallTwitchAsync<TwitchApiStreamInfoResponse>(suffixUrl);
                if (info == null)
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                else
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                return info?.Data?.FirstOrDefault();
            });
        }

        private async Task<T> CallTwitchAsync<T>(string suffix)
        {
            var token = await GetTokenAsync();

            twitchApi.DefaultRequestHeaders.Clear();
            twitchApi.DefaultRequestHeaders.Add("Client-Id", Environment.GetEnvironmentVariable("CLIENT_ID_TWITCH"));
            twitchApi.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Token}");

            var response = await twitchApi.GetAsync(suffix);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                logger.Error("Twitch API status code {0} - response body {1}", response.StatusCode, responseBody);
                return default;
            }

            return JsonSerializer.Deserialize<T>(responseBody);
        }

        private async Task<TwitchApiTokenResponse> GetTokenAsync()
        {
            return await cache.GetOrCreateAsync("TwitchService.GetTokenAsync", async e =>
            {
                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID_TWITCH");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET_TWITCH");
                var grantType = Environment.GetEnvironmentVariable("GRANT_TYPE_TWITCH");

                var urlSuffix = string.Format(options.Apis.Twitch.TokenSuffix, clientId, clientSecret, grantType);

                var response = await tokenTwitchApi.PostAsync(urlSuffix, null);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    logger.Error("Twitch API token status code {0} - response body {1}", response.StatusCode, responseBody);
                    return default;
                }

                var tokenCache = JsonSerializer.Deserialize<TwitchApiTokenResponse>(responseBody);
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenCache.ExpiresIn);
                return tokenCache;
            });
        }
    }
}
