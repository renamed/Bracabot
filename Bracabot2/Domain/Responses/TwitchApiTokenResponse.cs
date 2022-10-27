using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class TwitchApiTokenResponse
    {
        [JsonPropertyName("access_token")]        
        public string Token { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
