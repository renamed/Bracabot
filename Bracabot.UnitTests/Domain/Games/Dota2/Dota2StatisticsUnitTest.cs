using Bracabot.UnitTests.Support;
using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Responses;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Domain.Games.Dota2
{
    public class Dota2StatisticsUnitTest
    {
        [Fact]
        public void Dota2Statistics_ShouldReturnError_WhenGameCountDoesNotMatchVictoryPlusLoses()
        {
            // Arrange            
            var matches = new FakeList<DotaApiRecentMatchResponse>(new[]
            {
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = true
                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = false
                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 101,
                    RadiantWin = true
                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 101,
                    RadiantWin = false
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
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = true,
                    LobbyType = 1,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30

                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = false,
                    LobbyType = 1,
                    Kills = 20,
                    Deaths = 30,
                    Assists = 40
                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 101,
                    RadiantWin = true,
                    LobbyType = 1,
                    Kills = 30,
                    Deaths = 40,
                    Assists = 50
                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 101,
                    RadiantWin = false,
                    LobbyType = 7,
                    Kills = 40,
                    Deaths = 50,
                    Assists = 60
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
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = false,
                    LobbyType = 1,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30

                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = false,
                    LobbyType = 1,
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
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = true,
                    LobbyType = 1,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30

                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = true,
                    LobbyType = 1,
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
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = true,
                    LobbyType = 7,
                    Kills = 10,
                    Deaths = 20,
                    Assists = 30,
                    party_size =  2

                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 103,
                    RadiantWin = false,
                    LobbyType = 7,
                    Kills = 20,
                    Deaths = 30,
                    Assists = 40,
                    party_size =  2
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
