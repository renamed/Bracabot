using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class PeersCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly Mock<ITwitchService> twitchService;
        private readonly IOptions<SettingsOptions> options;

        public PeersCommandUnitTest()
        {
            dotaService = new Mock<IDotaService>();
            twitchService = new Mock<ITwitchService>();
            options = Options.Create(new SettingsOptions
            {
                ChannelName = "Imortal 2k"
            });
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenStreamerIsNotPlayingDota2()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(false);
            var peerCommand = new PeerCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await peerCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenArgsIsNull()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            var peerCommand = new PeerCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await peerCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Informe seu ID do dota ou nome da steam para visualizar sua estatísticas. Ex: !party [renamede] ou !party 420556150", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenArgsIsEmpty()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            var peerCommand = new PeerCommand(dotaService.Object, twitchService.Object, options);
            var args = Enumerable.Empty<string>().ToArray();

            // Act
            var result = await peerCommand.ExecuteAsync(args);

            // Assert
            Assert.Equal("Informe seu ID do dota ou nome da steam para visualizar sua estatísticas. Ex: !party [renamede] ou !party 420556150", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenPeerCannotBeFound()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetPeersAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((DotaApiPeersResponse)null);
            var args = new string[] { "aa" };
            var peerCommand = new PeerCommand(dotaService.Object, twitchService.Object, options);
            
            // Act
            var result = await peerCommand.ExecuteAsync(args);

            // Assert
            Assert.Equal("Não encontrei nenhum jogo com o identificador aa. Informe seu ID do dota ou nome da steam para visualizar sua estatísticas. Ex: !party [renamede] ou !party 420556150", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenPeerIsFound()
        {
            // Arrange
            twitchService.Setup(s => s.IsCurrentGameDota2()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetPeersAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new DotaApiPeersResponse
                {
                    Personaname = "Invoker de meteoro",
                    WithGames = 3,
                    WithWin = 2,
                    LastPlayed = (int)new DateTimeOffset(2022, 11, 2, 15, 32, 25, TimeSpan.Zero).ToUnixTimeSeconds()
                });
            var args = new string[] { "aa" };
            var peerCommand = new PeerCommand(dotaService.Object, twitchService.Object, options);

            // Act
            var result = await peerCommand.ExecuteAsync(args);

            // Assert
            Assert.Equal("Invoker de meteoro jogou 3 vez(es) com Imortal 2k, com 2 vitória(s). Último jogo em 02/11/2022 às 12:32:25. (Essas estatísticas podem não estar corretas se você desabilitou a opção de Exportar Dados da sua conta nas configurações do jogo de Dota 2.)", result);
        }
    }
}
