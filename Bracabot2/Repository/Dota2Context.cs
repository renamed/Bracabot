using Bracabot2.Domain.Games.Dota2;
using Microsoft.EntityFrameworkCore;

namespace Bracabot2.Repository
{
    public class Dota2Context : DbContext
    {
        public DbSet<Match> Matches { get; set; }

        public Dota2Context()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source=dota2.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>(e =>
            {
                e.ToTable("matches");
                e.HasKey(e => e.MatchId);
                e.Property(e => e.MatchId).ValueGeneratedOnAdd().HasColumnName("match_id");
                e.Property(e => e.PlayerSlot).HasColumnName("player_slot");
                e.Property(e => e.MatchResult).HasColumnName("match_result");
                e.Property(e => e.MatchType).HasColumnName("match_type");
                e.Property(e => e.HeroId).HasColumnName("hero_id");
                e.Property(e => e.StartTime).HasColumnName("start_time");
                e.Property(e => e.PartySize).HasColumnName("party_size");
                e.Property(e => e.EndTime).HasColumnName("end_time");
            });
        }
    }
}
