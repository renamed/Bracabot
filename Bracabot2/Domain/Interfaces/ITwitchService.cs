using Bracabot2.Domain.Responses;
using System.Threading.Tasks;

namespace Bracabot2.Domain.Interfaces
{
    public interface ITwitchService
    {
        Task<bool> EhOJogoDeDota();
        Task<TwitchApiChannelInfoResponse> GetChannelInfo();
    }
}
