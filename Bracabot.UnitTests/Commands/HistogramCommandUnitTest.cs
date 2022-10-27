using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Moq;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class HistogramCommandUnitTest
    {
        private readonly Mock<IDotaService> dotaService;
        private readonly Mock<ITwitchService> twitchService;
        private readonly string[] MmrArgs = new string[] { "1500" };

        public HistogramCommandUnitTest()
        {
            dotaService = new Mock<IDotaService>();
            twitchService = new Mock<ITwitchService>();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNotPlayingDota2()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(false);
            var histogramCommand = new HistogramCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await histogramCommand.ExecuteAsync(MmrArgs);

            // Assert
            Assert.Equal("Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenArgsIsNull()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            var histogramCommand = new HistogramCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await histogramCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal("Use !histograma SEU_MMR para saber como você se compara aos demais players de MMR. Ex: !histograma 3200", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenNoHeroNameIsNotInformed()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            var histogramCommand = new HistogramCommand(dotaService.Object, twitchService.Object);
            var args = Enumerable.Empty<string>().ToArray();

            // Act
            var result = await histogramCommand.ExecuteAsync(args);

            // Assert
            Assert.Equal("Use !histograma SEU_MMR para saber como você se compara aos demais players de MMR. Ex: !histograma 3200", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenMMRIsNotNumeric()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            var histogramCommand = new HistogramCommand(dotaService.Object, twitchService.Object);
            var mmr = new string[] { "aa" };

            // Act
            var result = await histogramCommand.ExecuteAsync(mmr);

            // Assert
            Assert.Equal("aa não parece um valor de MMR válido. Use !histograma SEU_MMR para saber como você se compara aos demais players de MMR. Ex: !histograma 3200", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenMMRIsBelowZero()
        {
            // Arrange
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            var histogramCommand = new HistogramCommand(dotaService.Object, twitchService.Object);
            var mmr = new string[] { "-5200" };

            // Act
            var result = await histogramCommand.ExecuteAsync(mmr);

            // Assert
            Assert.Equal("MMR negativo não parece um valor válido. Use !histograma SEU_MMR para saber como você se compara aos demais players de MMR. Ex: !histograma 3200", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenMMRIsNotImmortal()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetMmrBucketAsync()).ReturnsAsync(new DotaApiMmrBucketResponse
            {
                Mmr = new DotaApiMmrBucketRowsResponse
                {
                    Rows = new System.Collections.Generic.List<DotaApiMmrBucketRowsBinResponse>
                    {
                        new DotaApiMmrBucketRowsBinResponse
                        {
                            Bin = 0,
                            BinName = 0,
                            Count= 10,
                            CumulativeSum= 10
                        },
                        new DotaApiMmrBucketRowsBinResponse
                        {
                            Bin = 1,
                            BinName = 100,
                            Count= 20,
                            CumulativeSum= 30
                        },
                        new DotaApiMmrBucketRowsBinResponse
                        {
                            Bin = 2,
                            BinName = 200,
                            Count= 5,
                            CumulativeSum= 35
                        }
                    }
                }
            });
            var mmr = new string[] { "101" };

            var histogramCommand = new HistogramCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await histogramCommand.ExecuteAsync(mmr);

            // Assert
            Assert.Equal("Existem 20 jogadores com MMR entre 100 e 200 considerando todos os servers de Dota no mundo. Seu MMR é maior que (aproximadamente) 10 jogadores. Portanto, você está acima de 28,6% dos jogadores de Dota que calibraram seu MMR", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenMMRIsImmortal()
        {
            // Arrange
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            twitchService.Setup(s => s.EhOJogoDeDota()).ReturnsAsync(true);
            dotaService.Setup(s => s.GetMmrBucketAsync()).ReturnsAsync(new DotaApiMmrBucketResponse
            {
                Mmr = new DotaApiMmrBucketRowsResponse
                {
                    Rows = new System.Collections.Generic.List<DotaApiMmrBucketRowsBinResponse>
                    {
                        new DotaApiMmrBucketRowsBinResponse
                        {
                            Bin = 0,
                            BinName = 0,
                            Count= 10,
                            CumulativeSum= 10
                        },
                        new DotaApiMmrBucketRowsBinResponse
                        {
                            Bin = 1,
                            BinName = 100,
                            Count= 20,
                            CumulativeSum= 30
                        },
                        new DotaApiMmrBucketRowsBinResponse
                        {
                            Bin = 2,
                            BinName = 200,
                            Count= 5,
                            CumulativeSum= 35
                        }
                    }
                }
            });
            var mmr = new string[] { "201" };

            var histogramCommand = new HistogramCommand(dotaService.Object, twitchService.Object);

            // Act
            var result = await histogramCommand.ExecuteAsync(mmr);

            // Assert
            Assert.Equal("Existem 5 jogadores com MMR acima de 200 considerando todos os servers de Dota no mundo. Seu MMR é maior que (aproximadamente) 30 jogadores. Portanto, você está acima de 85,7% dos jogadores de Dota que calibraram seu MMR", result);
        }

    }
}
