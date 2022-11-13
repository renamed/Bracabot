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
        public ApisOptions Apis { get; set; }
    }

    public class ApisOptions
    {
        public DotaOptions Dota { get; set; }
        public TwitchOptions Twitch { get; set; }
    }

    public class DotaOptions
    {
        public string BaseAddress { get; set; }
        public string Players { get; set; }
        public string HeroStatisticsForPlayer { get; set; }
        public string MmrBuckets { get; set; }
        public string Peers { get; set; }
        public string Matches { get; set; }
    }

    public class TwitchOptions
    {
        public string BaseAddress { get; set; }
        public string BaseAddressToken { get; set; }
        public string TokenSuffix { get; set; }
        public string StreamInfo { get; set; }
    }
}