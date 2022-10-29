using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class HeroCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly Mock<ITwitchService> twitchService;
        private readonly IOptions<SettingsOptions> options;
        private readonly string[] HeroNameArg = new string[] { "aa" };

        public HeroCommandUnitTest()
        {
            dotaService = new Mock<IDotaService>();
            twitchService = new Mock<ITwitchService>();
            options = Options.Create(new SettingsOptions
            {
                ChannelName = "XXX"
            });
        }
        #region ExecuteAsync
        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNotPlayingDota2()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(false);
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await heroCommand.ExecuteAsync(HeroNameArg);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenArgsIsNull()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await heroCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Informe um herói para que eu possa lhe mostrar algumas estatísticas. Por exemplo, !heroi aa", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNoHeroNameIsNotInformed()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);
            var args = Enumerable.Empty<string>().ToArray();

            // Act
            var result = await heroCommand.ExecuteAsync(args);

            // Assert
            Assert.Equal("Informe um herói para que eu possa lhe mostrar algumas estatísticas. Por exemplo, !heroi aa", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNoHeroNameIsUnknown()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetIdAsync(It.IsAny<string>())).ReturnsAsync((string)null);
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);
            
            // Act
            var result = await heroCommand.ExecuteAsync(HeroNameArg);

            // Assert
            Assert.Equal("Não conheço o herói chamado aa. Você digitou o nome certo?", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenDotaApiReturnsError()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetIdAsync(It.IsAny<string>())).ReturnsAsync("bb");
            dotaService.Setup(s => s.GetHeroStatisticsForPlayerAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((DotaApiHeroResponse)null);
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await heroCommand.ExecuteAsync(HeroNameArg);

            // Assert
            Assert.Equal("Achei um bug e não consigo mostrar as estatísticas do hero", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenDotaApiReturnsError2()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetIdAsync(It.IsAny<string>())).ReturnsAsync("bb");
            dotaService.Setup(s => s.GetHeroStatisticsForPlayerAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new DotaApiHeroResponse());
            dotaService.Setup(s => s.GetNameAsync(It.IsAny<string>())).ReturnsAsync((string)null);
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await heroCommand.ExecuteAsync(HeroNameArg);

            // Assert
            Assert.Equal("Achei um bug e não consigo mostrar as estatísticas do hero", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenEverythingWorrksFine()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetIdAsync(It.IsAny<string>())).ReturnsAsync("bb");
            dotaService.Setup(s => s.GetHeroStatisticsForPlayerAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new DotaApiHeroResponse());
            dotaService.Setup(s => s.GetNameAsync(It.IsAny<string>())).ReturnsAsync("Hero");
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await heroCommand.ExecuteAsync(HeroNameArg);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, nunca jogou com Hero. Como oponente, nunca jogou contra Hero.", result);
        }
#endregion

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenNeverPlayedAndNoAlliesAndNoEnemies()
        {
            // Arrange
            var dotaApiResponse = new DotaApiHeroResponse();
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);
            
            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, nunca jogou com Hero. Como oponente, nunca jogou contra Hero.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedOnceAndLostAndNoAlliesAndNoEnemies()
        {
            // Arrange
            var dotaApiResponse = new DotaApiHeroResponse
            {
                Games = 1,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX já jogou com Hero uma única vez e perdeu. Como aliado, nunca jogou com Hero. Como oponente, nunca jogou contra Hero. O último jogo com o heroi foi em 26/10/2022 às 12:00:00.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedOnceAndWonAndNoAlliesAndNoEnemies()
        {
            // Arrange
            var dotaApiResponse = new DotaApiHeroResponse
            {
                Games = 1,
                Win = 1,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX já jogou com Hero uma única vez e ganhou. Como aliado, nunca jogou com Hero. Como oponente, nunca jogou contra Hero. O último jogo com o heroi foi em 26/10/2022 às 12:00:00.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedMoreThanOneTimeAndLostAndNoAlliesAndNoEnemies()
        {
            // Arrange
            var dotaApiResponse = new DotaApiHeroResponse
            {
                Games = 2,
                Win = 0,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX já jogou com Hero 2 vezes e perdeu todas. Como aliado, nunca jogou com Hero. Como oponente, nunca jogou contra Hero. O último jogo com o heroi foi em 26/10/2022 às 12:00:00.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedMoreThanOneTimeAndOneVictoryAndNoAlliesAndNoEnemies()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                Games = 2,
                Win = 1,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX já jogou com Hero 2 vezes com apenas uma vitória (50,0%). Como aliado, nunca jogou com Hero. Como oponente, nunca jogou contra Hero. O último jogo com o heroi foi em 26/10/2022 às 12:00:00.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedMoreThanOneTimeAndMoreThanOneVictoryAndNoAlliesAndNoEnemies()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                Games = 10,
                Win = 2,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX já jogou com Hero 10 vezes com 2 vitórias (20,0%). Como aliado, nunca jogou com Hero. Como oponente, nunca jogou contra Hero. O último jogo com o heroi foi em 26/10/2022 às 12:00:00.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedOneTimeAndWonInAllies()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                WithGames = 1,
                WithWin = 1,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, foi um único jogo com vitória. Como oponente, nunca jogou contra Hero.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedOneTimeAndLostInAllies()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                WithGames = 1,
                WithWin = 0,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, foi um único jogo com derrota. Como oponente, nunca jogou contra Hero.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedMoreThanOneTimeAndWonOnceInAllies()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                WithGames = 2,
                WithWin = 1,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, foram 2 jogos com 1 vitória (50,0%). Como oponente, nunca jogou contra Hero.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedMoreThanOneTimeAndWonMoreThanOnceInAllies()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                WithGames = 4,
                WithWin = 2,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, foram 4 jogos com 2 vitórias (50,0%). Como oponente, nunca jogou contra Hero.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedOneTimeAndWonInEnemy()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                AgainstGames = 1,
                AgainstWin = 1,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, nunca jogou com Hero. Como oponente, foi um único jogo com vitória.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedOneTimeAndLostInEnemy()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                AgainstGames = 1,
                AgainstWin = 0,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, nunca jogou com Hero. Como oponente, foi um único jogo com derrota.", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedMoreThanOneTimeAndWonOnceInEnemy()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                AgainstGames = 2,
                AgainstWin = 1,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, nunca jogou com Hero. Como oponente, foram 2 jogos com 1 vitória (50,0%).", result);
        }

        [Fact]
        public void WriteHeroMessage_ShouldReturnMesage_WhenPlayedMoreThanOneTimeAndWonMoreThanOnceInEnemy()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var dotaApiResponse = new DotaApiHeroResponse
            {
                AgainstGames = 4,
                AgainstWin = 2,
                LastPlayed = (int)new DateTimeOffset(2022, 10, 26, 15, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds()
            };
            var localizedName = "Hero";
            var heroCommand = new HeroCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = heroCommand.WriteHeroMessage(dotaApiResponse, localizedName);

            // Assert
            Assert.Equal("XXX nunca jogou com Hero. Como aliado, nunca jogou com Hero. Como oponente, foram 4 jogos com 2 vitórias (50,0%).", result);
        }
    }
}
