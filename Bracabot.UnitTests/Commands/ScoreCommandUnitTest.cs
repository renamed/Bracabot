using Bracabot2.Commands;
using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Match = Bracabot2.Domain.Games.Dota2.Match;

namespace Bracabot.UnitTests.Commands
{
    public class ScoreCommandUnitTest
    {
        private readonly Mock<IDotaRepository> dotaRepository;
        private readonly Mock<ITwitchService> twitchService;

        public ScoreCommandUnitTest()
        {
            dotaRepository = new Mock<IDotaRepository>();
            twitchService = new Mock<ITwitchService>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNotOnline()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync((TwitchApiStreamInfoNodeResponse)null);

            var performanceCommand = new ScoreCommand(twitchService.Object, dotaRepository.Object);

            // Act
            var result = await performanceCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Streamer não está online", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenGameIsNotDota2()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = "ss"
                });

            var scoreCommand = new ScoreCommand(twitchService.Object, dotaRepository.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenGetLastMatchesAsyncReturnsError()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = Consts.Twitch.DOTA_2_ID
                });

            dotaRepository.Setup(s => s.GetLastMatchesAsync(It.IsAny<DateTime>()))
                .ReturnsAsync((IEnumerable<Match>)null);

            var scoreCommand = new ScoreCommand(twitchService.Object, dotaRepository.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Nenhuma partida encontrada. Seria essa a primeira do dia? ;)", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenAllMatchesAreBefore5HoursAgo()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = Consts.Twitch.DOTA_2_ID
                });

            dotaRepository.Setup(s => s.GetLastMatchesAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(Enumerable.Empty<Match>());

            var scoreCommand = new ScoreCommand(twitchService.Object, dotaRepository.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Nenhuma partida encontrada. Seria essa a primeira do dia? ;)", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenAllMatchesAreBefore5HoursAgo()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = Consts.Twitch.DOTA_2_ID
                });

            dotaRepository.Setup(s => s.GetLastMatchesAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new[]
            {
                new Match
                {
                    MatchResult = MatchResult.Win,
                    Kills = 1,
                    Deaths = 1,
                    Assists = 1,
                    StartTime = DateTime.UtcNow.AddHours(-3)
                },
                new Match
                {
                    MatchResult = MatchResult.Lose,
                    Kills = 1,
                    Deaths = 1,
                    Assists = 1,
                    StartTime = DateTime.UtcNow.AddHours(-6)
                }

            });

            var scoreCommand = new ScoreCommand(twitchService.Object, dotaRepository.Object);

            // Act
            var result = await scoreCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("J = 2 --- V -> 1 --- D -> 1", result);
        }
    }
}
