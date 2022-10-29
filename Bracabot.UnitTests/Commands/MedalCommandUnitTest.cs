using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class MedalCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly Mock<ITwitchService> twitchService;
        private readonly IOptions<SettingsOptions> options;

        public MedalCommandUnitTest()
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
            var medalCommand = new MedalCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await medalCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenMedalReturnsNull()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetMedalAsync(It.IsAny<int>())).ReturnsAsync((string)null);
            dotaService.Setup(s => s.GetPlayerAsync(It.IsAny<string>())).ReturnsAsync(new DotaApiPlayerResponse
            {
                RankTier = 31
            });
            var medalCommand = new MedalCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await medalCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Deu bug, não encontrei a medalha informada pela Valve =/", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenMedalIsNotImmortal()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetMedalAsync(It.IsAny<int>())).ReturnsAsync("Medalha");
            dotaService.Setup(s => s.GetPlayerAsync(It.IsAny<string>())).ReturnsAsync(new DotaApiPlayerResponse
            {
                RankTier = 31
            });
            var medalCommand = new MedalCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await medalCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Medalha 1", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenMedalIsImmortal()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetMedalAsync(It.IsAny<int>())).ReturnsAsync("Imorrível");
            dotaService.Setup(s => s.GetPlayerAsync(It.IsAny<string>())).ReturnsAsync(new DotaApiPlayerResponse
            {
                RankTier = 80,
                LeaderboardRank = 5
            });
            var medalCommand = new MedalCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await medalCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Imorrível rank 5", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenMedalIsNotImmortalButRanktier()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetMedalAsync(It.IsAny<int>())).ReturnsAsync("Xaxa");
            dotaService.Setup(s => s.GetPlayerAsync(It.IsAny<string>())).ReturnsAsync(new DotaApiPlayerResponse
            {
                RankTier = 75,
                LeaderboardRank = 3745
            });
            var medalCommand = new MedalCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await medalCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Rank 3745", result);
        }
    }
}
