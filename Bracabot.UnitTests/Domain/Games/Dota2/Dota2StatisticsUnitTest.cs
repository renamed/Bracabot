using Bracabot.UnitTests.Support;
using Bracabot2.Domain.Games.Dota2;
using Xunit;

namespace Bracabot.UnitTests.Domain.Games.Dota2
{
    public class Dota2StatisticsUnitTest
    {
        [Fact]
        public void Dota2Statistics_ShouldReturnError_WhenGameCountDoesNotMatchVictoryPlusLoses()
        {
            // Arrange            
            var matches = new FakeList<Match>(new[]
            {
                new Match
                {
                    MatchResult = MatchResult.Win
                },
                new Match
                {
                    MatchResult = MatchResult.Lose
                },
                new Match
                {
                    MatchResult = MatchResult.Lose
                },
                new Match
                {
                    MatchResult = MatchResult.Win
                }
            });

            // Act
            var result = new Dota2Statistics(matches);

            // Assert
            Assert.True(result.HasError);
            Assert.Equal("As contas do Renamede não estão corretas, avisa pra ele!", result.ErrorDescription);
        }

        [Fact]
        public void Dota2Statistics_ShouldNotComputeMMR_WhenLobbyTypeIsNotRanked()
        {
            // Arrange
            var matches = new []
            {
                new Match
                {
                    MatchResult = MatchResult.Win,
                    MatchType = MatchType.Normal,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30,
                    PartySize = 1

                },
                new Match
                {
                    MatchResult = MatchResult.Lose,
                    MatchType = MatchType.Normal,
                    Kills = 20,
                    Deaths = 30,
                    Assists = 40,
                    PartySize = 1
                },
                new Match
                {
                    MatchResult = MatchResult.Lose,
                    MatchType = MatchType.Normal,
                    Kills = 30,
                    Deaths = 40,
                    Assists = 50,
                    PartySize = 1
                },
                new Match
                {
                    MatchResult = MatchResult.Win,
                    MatchType = MatchType.Ranked,
                    Kills = 40,
                    Deaths = 50,
                    Assists = 60,
                    PartySize = 1
                }
            };

            // Act
            var result = new Dota2Statistics(matches);

            // Assert
            Assert.Equal(4, result.Games);
            Assert.Equal(2, result.Victories);
            Assert.Equal(2, result.Defeats);
            Assert.Equal(30, result.Mmr);
            Assert.Equal(25, result.AvgK);
            Assert.Equal(35, result.AvgD);
            Assert.Equal(45, result.AvgA);
        }

        [Fact]
        public void Dota2Statistics_ShouldDisplayNenhumaInVictory_WhenThereAreNoWins()
        {
            // Arrange
            var matches = new []
            {
                new Match
                {
                    MatchResult = MatchResult.Lose,
                    MatchType = MatchType.Normal,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30

                },
                new Match
                {
                    MatchResult = MatchResult.Lose,
                    MatchType = MatchType.Normal,
                    Kills = 20,
                    Deaths = 30,
                    Assists = 40
                }
            };

            // Act
            var result = new Dota2Statistics(matches);

            // Assert
            Assert.Equal(2, result.Games);
            Assert.Equal(0, result.Victories);
            Assert.Equal(2, result.Defeats);
            Assert.Equal(0, result.Mmr);
            Assert.Equal(15, result.AvgK);
            Assert.Equal(25, result.AvgD);
            Assert.Equal(35, result.AvgA);
        }

        [Fact]
        public void Dota2Statistics_ShouldDisplayNenhumaInDefeats_WhenThereAreOnlyWins()
        {
            // Arrange
            var matches = new []
            {
                new Match
                {
                    MatchResult = MatchResult.Win,
                    MatchType = MatchType.Normal,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30

                },
                new Match
                {
                    MatchResult = MatchResult.Win,
                    MatchType = MatchType.Normal,
                    Kills = 20,
                    Deaths = 30,
                    Assists = 40
                }
            };

            // Act
            var result = new Dota2Statistics(matches);

            // Assert
            Assert.Equal(2, result.Games);
            Assert.Equal(2, result.Victories);
            Assert.Equal(0, result.Defeats);
            Assert.Equal(0, result.Mmr);
            Assert.Equal(15, result.AvgK);
            Assert.Equal(25, result.AvgD);
            Assert.Equal(35, result.AvgA);
        }

        [Fact]
        public void Dota2Statistics_ShouldComputeMMR_WhenPlayerIsInAParty()
        {
            // Arrange
            var matches = new []
            {
                new Match
                {
                    MatchResult = MatchResult.Win,
                    MatchType = MatchType.Ranked,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30,
                    PartySize = 2

                },
                new Match
                {
                    MatchResult = MatchResult.Win,
                    MatchType = MatchType.Ranked,
                    Kills = 20,
                    Deaths = 30,
                    Assists = 40,
                    PartySize =  2
                }
            };

            // Act
            var result = new Dota2Statistics(matches);

            // Assert
            Assert.Equal(2, result.Games);
            Assert.Equal(2, result.Victories);
            Assert.Equal(0, result.Defeats);
            Assert.Equal(40, result.Mmr);
            Assert.Equal(15, result.AvgK);
            Assert.Equal(25, result.AvgD);
            Assert.Equal(35, result.AvgA);
        }
    }
}
