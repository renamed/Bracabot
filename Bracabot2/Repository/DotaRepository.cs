using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bracabot2.Repository
{
    public class DotaRepository : IDotaRepository
    {
        private readonly Dota2Context context;

        public DotaRepository(Dota2Context context)
        {
            this.context = context;
        }

        public async Task<Match> GetLastMatchAsync()
        {
            return await context.Matches.OrderByDescending(d => d.EndTime).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Match>> GetLastMatchesAsync(int limit = 50)
        {
            return await context.Matches.OrderByDescending(d => d.EndTime).Take(limit).ToArrayAsync();
        }

        public Task<int> AddAsync(Match match)
        {
            context.Matches.Add(match);
            return context.SaveChangesAsync();
        }

        public async Task AddAsync(IEnumerable<Match> matches)
        {
            var tasks = new List<Task<int>>();
            matches.ToList().ForEach(match => tasks.Add(AddAsync(match)));
            await Task.WhenAll(tasks);
        }
    }
}
