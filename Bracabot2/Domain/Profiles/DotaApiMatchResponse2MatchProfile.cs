using AutoMapper;
using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Responses;
using MatchType = Bracabot2.Domain.Games.Dota2.MatchType;

namespace Bracabot2.Domain.Profiles
{
    public class DotaApiMatchResponse2MatchProfile : Profile
    {
        public DotaApiMatchResponse2MatchProfile()
        {
            CreateMap<DotaApiMatchResponse, Match>()
                .ForMember(
                    dest => dest.PlayerSlot,
                    act => act.MapFrom(src => src.PlayerSlot < 100 ? MatchSlot.Radiant : MatchSlot.Dire) 
                )
                .ForMember(
                    dest => dest.MatchResult,
                    act => act.MapFrom(src => MapMatchResult(src))
                )
                .ForMember(
                    dest => dest.MatchType,
                    act => act.MapFrom(src => MapMatchType(src))
                )
                .ForMember(
                    dest => dest.StartTime,
                    act => act.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.UnixStartTime)
                                                .DateTime.ToUniversalTime())
                )
                .ForMember(
                    dest => dest.EndTime,
                    act => act.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.UnixStartTime)
                                                .AddSeconds(src.Duration).DateTime.ToUniversalTime())
                );
            
        }

        private static MatchResult MapMatchResult(DotaApiMatchResponse p)
        {
            return p.PlayerSlot < 100 && p.RadiantWin || p.PlayerSlot >= 100 && !p.RadiantWin
                    ? MatchResult.Win
                    : MatchResult.Lose;
        }

        private static MatchType MapMatchType(DotaApiMatchResponse p)
        {
            if (p.LobbyType == 7)
                return MatchType.Ranked;
            return MatchType.Unknown;
        }
    }
}
