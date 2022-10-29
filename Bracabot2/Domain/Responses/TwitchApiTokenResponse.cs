using Bracabot2.Domain.Support;
using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class TwitchApiTokenResponse
    {
        private readonly DateTime created;

        public bool IsValid => created.AddMinutes(ExpiresIn) <= DateTime.UtcNow.AddSeconds(-Consts.Clients.TWITCH_API_TOKEN_TIMEOUT);

        [JsonPropertyName("access_token")]        
        public string Token { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        public TwitchApiTokenResponse()
        {
            created = DateTime.UtcNow;
        }
    }
}
