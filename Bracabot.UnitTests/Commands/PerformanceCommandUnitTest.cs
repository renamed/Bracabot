using Bracabot2.Commands;
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
    public class PerformanceCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly Mock<ITwitchService> twitchService;
        private readonly Mock<IDotaRepository> dotaRepository;

        public PerformanceCommandUnitTest()
        {
            dotaService = new Mock<IDotaService>();
            twitchService = new Mock<ITwitchService>();
            dotaRepository = new Mock<IDotaRepository>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNotOnline()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync((TwitchApiStreamInfoNodeResponse)null);

            var performanceCommand = new PerformanceCommand(dotaService.Object, twitchService.Object, dotaRepository.Object);

            // Act
            var result = await performanceCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Streamer não está online", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNotPlayingDota2()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = "ss"
                });
            var performanceCommand = new PerformanceCommand(dotaService.Object, twitchService.Object, dotaRepository.Object);

            // Act
            var result = await performanceCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenGetLastMatchesAsyncReturnNull()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = Consts.Twitch.DOTA_2_ID
                });
            dotaRepository.Setup(s => s.GetLastMatchesAsync(It.IsAny<DateTime>()))
                .ReturnsAsync((IEnumerable<Match>)null);

            var performanceCommand = new PerformanceCommand(dotaService.Object, twitchService.Object, dotaRepository.Object);

            // Act
            var result = await performanceCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Nenhuma partida encontrada. Seria essa a primeira? ;)", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenGetLastMatchesAsyncReturnEmpty()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = Consts.Twitch.DOTA_2_ID
                });
            dotaRepository.Setup(s => s.GetLastMatchesAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(Enumerable.Empty<Match>());

            var performanceCommand = new PerformanceCommand(dotaService.Object, twitchService.Object, dotaRepository.Object);

            // Act
            var result = await performanceCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Nenhuma partida encontrada. Seria essa a primeira? ;)", result);
        }
    }
}
