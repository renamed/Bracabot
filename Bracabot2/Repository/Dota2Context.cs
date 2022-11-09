using Bracabot2.Domain.Games.Dota2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using MatchType = Bracabot2.Domain.Games.Dota2.MatchType;

namespace Bracabot2.Repository
{
    public class Dota2Context : DbContext
    {
        public Dota2Context(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Match> Matches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONN_STR"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>(e =>
            {
                e.ToTable("matches");
                e.HasKey(e => e.Id);
                e.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");
                e.Property(e => e.MatchId).HasColumnName("match_id");
                e.Property(e => e.PlayerSlot).HasColumnName("player_slot").HasConversion(new EnumToStringConverter<MatchSlot>());                
                e.Property(e => e.MatchResult).HasColumnName("match_result").HasConversion(new EnumToStringConverter<MatchResult>());
                e.Property(e => e.MatchType).HasColumnName("match_type").HasConversion(new EnumToStringConverter<MatchType>());
                e.Property(e => e.HeroId).HasColumnName("hero_id");
                e.Property(e => e.StartTime).HasColumnName("start_time");
                e.Property(e => e.EndTime).HasColumnName("end_time");
                e.HasIndex(e => e.EndTime);
                e.Property(e => e.PartySize).HasColumnName("party_size");
                e.Property(e => e.Deaths).HasColumnName("deaths");
                e.Property(e => e.Assists).HasColumnName("assists");
                e.Property(e => e.Kills).HasColumnName("kills");
            });
        }
    }
}
