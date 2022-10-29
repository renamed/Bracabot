using System.Threading.Tasks;
using Xunit;

namespace Bracabot2.Commands
{
    public class RecalibrationCommandUnitTest
    {
        [Fact]
        public async Task ExecuteAsync_ShouldDisplayRecalibrationInfo()
        {
            // Arrange
            var command = new RecalibrationCommand();

            // Act
            var result = await command.ExecuteAsync(null);

            // Assert
            Assert.Equal("Terminei de recarregar em 22/10/2022 com 4 Vitórias e 6 Derrotas. Medalha alcançada: Cruzado 4", result);
        }
    }
}
