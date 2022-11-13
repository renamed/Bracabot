using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class FreedomCommandUnitTest
    {
        private readonly Mock<IDotaRepository> dotaRepository;
        private readonly Mock<ITwitchService> twitchService;
        private readonly IOptions<SettingsOptions> options;

        public FreedomCommandUnitTest()
        {
            dotaRepository = new Mock<IDotaRepository>();
            twitchService = new Mock<ITwitchService>();
            options = Options.Create(new SettingsOptions());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessageError_WhenTwitchApiIsUnavailable()
        {
            // Arrange
            twitchService.Setup(x => x.GetStreamInfo())
                .ReturnsAsync((TwitchApiStreamInfoNodeResponse)null);

            var freedomCommand = new FreedomCommand(dotaRepository.Object, twitchService.Object);

            // Act
            var response = await freedomCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Streamer não está online", response);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenDotaApiReturns()
        {
            // Arrange
            twitchService.Setup(s => s.GetStreamInfo())
                .ReturnsAsync(new TwitchApiStreamInfoNodeResponse
                {
                    GameId = "aaa"
                });

            dotaRepository.Setup(x => x.GetLastMatchAsync())
                .ReturnsAsync(
                    new Bracabot2.Domain.Games.Dota2.Match
                    {
                        StartTime = DateTime.UtcNow.AddDays(-10),
                        EndTime = DateTime.UtcNow.AddDays(-10).AddSeconds(1)
                    });

            var freedomCommand = new FreedomCommand(dotaRepository.Object, twitchService.Object);

            // Act
            var response = await freedomCommand.ExecuteAsync(null);

            // Assert
            Assert.Contains("Estamos há 1 semana, 2 dias, 23 horas, 59 minutos, 59 segundos sem o jogo de Dota bracubiClap", response);
        }
    }
}
