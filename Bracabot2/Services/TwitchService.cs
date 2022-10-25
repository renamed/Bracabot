using Bracabot2.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracabot2.Services
{
    public class TwitchService : WebApiServiceBase
    {
        public async Task<TwitchApiChannelInfoResponse> GetChannelInfo()
        {
            var twitchBroadcastId = Environment.GetEnvironmentVariable("TWITCH_BROADCAST_ID");
            return await CallTwitchAsync<TwitchApiChannelInfoResponse>($"/channels?broadcaster_id={twitchBroadcastId}");
        }

        public Task<bool> EhOJogoDeDota()
        {
            return Task.FromResult(true);
            // var channelInfo = await GetChannelInfo();
            // return channelInfo.Data.FirstOrDefault()?.GameId == "29595";
        }

        private async Task<T> CallTwitchAsync<T>(string suffix)
        {
            var headers = new Dictionary<string, string>
            {
                {"Client-Id", Environment.GetEnvironmentVariable("CLIENT_ID_TWITCH") },
                {"Authorization", $"Bearer {Environment.GetEnvironmentVariable("AUTHORIZATION_TWITCH")}" },
            };

            return await CallApiAsync<T>($"https://api.twitch.tv/helix{suffix}", headers);            
        }
    }
}
