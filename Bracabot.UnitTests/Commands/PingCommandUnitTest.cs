using Bracabot2.Commands;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class PingCommandUnitTest
    {
        [Fact]
        public async Task ExecuteAsync_ShouldAnswerPingCommand()
        {
            // Arrange
            var command = new PingCommand();

            // Act
            var result = await command.ExecuteAsync(null);

            // Assert
            Assert.Equal("Sim!", result);
        }
    }
}
