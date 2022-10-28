using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class PerformanceCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly Mock<ITwitchService> twitchService;
        private readonly IOptions<SettingsOptions> options;

        public PerformanceCommandUnitTest()
        {
            dotaService = new Mock<IDotaService>();
            twitchService = new Mock<ITwitchService>();
            options = Options.Create(new SettingsOptions());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNotPlayingDota2()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(false);
            var performanceCommand = new PerformanceCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await performanceCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenGetRecentMatchesAsyncReturnNull()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetRecentMatchesAsync(It.IsAny<string>())).ReturnsAsync((IEnumerable<DotaApiRecentMatchResponse>)null);
            var performanceCommand = new PerformanceCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await performanceCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("A API do Dota retornou um erro. Não consegui ver as últimas partidas", result);
        }        
    }
}
