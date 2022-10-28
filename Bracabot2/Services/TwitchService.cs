using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;

namespace Bracabot2.Services
{
    public class TwitchService : ITwitchService
    {
        private readonly IWebApiService webApiService;
        private readonly SettingsOptions options;

        public TwitchService(IWebApiService webApiService, IOptions<SettingsOptions> options)
        {
            this.webApiService = webApiService;
            this.options = options.Value;
        }

        public async Task<TwitchApiChannelInfoResponse> GetChannelInfo()
        {
            var twitchBroadcastId = options.TwitchBroadcastId;
            return await CallTwitchAsync<TwitchApiChannelInfoResponse>($"/channels?broadcaster_id={twitchBroadcastId}");
        }

        public async Task<bool> IsCurrentGameDota2()
        {            
            var channelInfo = await GetChannelInfo();
            return channelInfo.Data.FirstOrDefault()?.GameId == Consts.Twitch.DOTA_2_ID;
        }

        private async Task<T> CallTwitchAsync<T>(string suffix)
        {
            var token = await GetTokenAsync();

            var headers = new Dictionary<string, string>
            {
                {"Client-Id", Environment.GetEnvironmentVariable("CLIENT_ID_TWITCH") },
                {"Authorization", $"Bearer {token.Token}" },
            };

            return await webApiService.GetAsync<T>($"https://api.twitch.tv/helix{suffix}", headers);            
        }

        private async Task<TwitchApiTokenResponse> GetTokenAsync()
        {
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID_TWITCH");
            var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET_TWITCH");
            var grantType = Environment.GetEnvironmentVariable("GRANT_TYPE_TWITCH");

            var url = $"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type={grantType}";
            return await webApiService.PostAsync<TwitchApiTokenResponse>(url, null);
        }
    }
}
