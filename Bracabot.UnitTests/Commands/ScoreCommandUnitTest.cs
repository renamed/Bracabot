using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class ScoreCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly Mock<ITwitchService> twitchService;

        public ScoreCommandUnitTest()
        {
            dotaService = new Mock<IDotaService>();
            twitchService = new Mock<ITwitchService>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenGameIsNotDota2()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(false);
            var scoreCommand = new ScoreCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenGetRecentMatchesAsyncReturnsError()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetRecentMatchesAsync(It.IsAny<string>())).ReturnsAsync((IEnumerable<DotaApiRecentMatchResponse>) null);
            var scoreCommand = new ScoreCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("A API do Dota retornou um erro. Não consegui ver as últimas partidas", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenAllMatchesAreBefore5HoursAgo()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetRecentMatchesAsync(It.IsAny<string>())).ReturnsAsync(new[]
            {
                new DotaApiRecentMatchResponse
                {
                    UnixStartTime = (int)new DateTimeOffset(DateTime.UtcNow.AddHours(-5)).ToUnixTimeSeconds()
                },
                new DotaApiRecentMatchResponse
                {
                    UnixStartTime = (int)new DateTimeOffset(DateTime.UtcNow.AddHours(-10)).ToUnixTimeSeconds()
                }

            });
            var scoreCommand = new ScoreCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Não achei partidas recentes", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenAllMatchesAreBefore5HoursAgo()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetRecentMatchesAsync(It.IsAny<string>())).ReturnsAsync(new[]
            {
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 1,
                    RadiantWin = true,
                    Kills = 1,
                    Deaths = 1,
                    Assists = 1,
                    UnixStartTime = (int)new DateTimeOffset(DateTime.UtcNow.AddHours(-3)).ToUnixTimeSeconds()
                },
                new DotaApiRecentMatchResponse
                {
                    PlayerSlot = 100,
                    RadiantWin = true,
                    Kills = 1,
                    Deaths = 1,
                    Assists = 1,
                    UnixStartTime = (int)new DateTimeOffset(DateTime.UtcNow.AddHours(-6)).ToUnixTimeSeconds()
                }

            });
            var scoreCommand = new ScoreCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("J = 2 --- V -> 1 --- D -> 1 --- Saldo 0", result);
        }
    }
}
