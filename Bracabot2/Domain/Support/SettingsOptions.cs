using Bracabot2.Domain.DataStructure;

namespace Bracabot2.Domain.Support
{
    public class SettingsOptions
    {
        public string DotaId { get; set; }
        public string ChannelName { get; set; }
        public string TwitchBroadcastId { get; set; }
        public string IpTwitchIrc { get; set; }
        public string PortTwitchIrc { get; set; }
        public CaseInsensitiveDictionary<string> HeroesFromId { get; set; }
        public CaseInsensitiveDictionary<int> HeroesFromName { get; set; }
        public Dictionary<int, string> Medals { get; set; }
    }
}