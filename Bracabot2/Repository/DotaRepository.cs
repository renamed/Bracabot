using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bracabot2.Repository
{
    public class DotaRepository : IDotaRepository
    {
        private readonly DotaContext context;

        public DotaRepository(DotaContext context)
        {
            this.context = context;
        }

        public async Task<Match> GetLastMatchAsync()
        {
            return await context.Matches.OrderByDescending(d => d.EndTime).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Match>> GetLastMatchesAsync(DateTime fromEndTime)
        {
            return await context.Matches.Where(d => d.EndTime >= fromEndTime).ToArrayAsync();
        }

        public Task<int> AddAsync(Match match)
        {
            context.Matches.Add(match);
            return context.SaveChangesAsync();
        }

        public async Task<int> AddIfNotExistsAsync(IEnumerable<Match> matches)
        {
            var ids = matches.Select(x => x.MatchId);
            var existingMatches = (await context.Matches.Where(x => ids.Contains(x.MatchId)).ToListAsync()).Select(x => x.MatchId).ToHashSet();

            matches.ToList().ForEach(match =>
            {
                if (!existingMatches.Contains(match.MatchId))
                {
                    context.Matches.Add(match);
                }
            });
            return await context.SaveChangesAsync();
        }
    }
}
