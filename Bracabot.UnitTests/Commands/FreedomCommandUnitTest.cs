using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class FreedomCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly IOptions<SettingsOptions> options;

        public FreedomCommandUnitTest()
        {
            dotaService = new Mock<IDotaService>();
            options = Options.Create(new SettingsOptions());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessageError_WhenDotaApiIsUnavailable()
        {
            // Arrange
            dotaService.Setup(x => x.GetRecentMatchesAsync(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<DotaApiRecentMatchResponse>)null);

            var freedomCommand = new FreedomCommand(dotaService.Object, options);

            // Act
            var response = await freedomCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("A API do Dota retornou um erro. Não consegui ver as últimas partidas", response);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenDotaApiReturns()
        {
            // Arrange
            dotaService.Setup(x => x.GetRecentMatchesAsync(It.IsAny<string>()))
                .ReturnsAsync(new DotaApiRecentMatchResponse[]
                {
                    new DotaApiRecentMatchResponse
                    {
                        UnixStartTime = (int)DateTimeOffset.UtcNow.AddDays(-10).ToUnixTimeSeconds(),
                        Duration = 1
                    }
                });

            var freedomCommand = new FreedomCommand(dotaService.Object, options);

            // Act
            var response = await freedomCommand.ExecuteAsync(null);

            // Assert
            Assert.Contains("Estamos há 1 semana, 2 dias, 23 horas, 59 minutos, 59 segundos sem o jogo de Dota bracubiClap", response);
        }
    }
}
