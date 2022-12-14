using Bracabot2.Domain.Games.Dota2;

namespace Bracabot2.Domain.Interfaces
{
    public interface IDotaRepository
    {
        Task<int> AddAsync(Match match);
        Task<int> AddIfNotExistsAsync(IEnumerable<Match> matches);
        Task<Match> GetLastMatchAsync();
        Task<IEnumerable<Match>> GetLastMatchesAsync(DateTime fromEndTime);
    }
}
