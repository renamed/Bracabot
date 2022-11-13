using Bracabot2.Commands;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class CommandsCommandUnitTest
    {
        [Fact]
        public async Task ExecuteAsync_ShouldListAllCommands()
        {
            var command = new CommandsCommand();            

            // Act
            var response = await command.ExecuteAsync(null);

            // Assert
            Assert.Equal("!aproveitamento !placar !heroi !medalha !tai !comandos !histograma !info !liberdade !recalibracao !peer", response);
        }
    }
}
