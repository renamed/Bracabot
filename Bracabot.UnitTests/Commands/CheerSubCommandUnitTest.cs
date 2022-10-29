using Bracabot2.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Bracabot.UnitTests.Commands
{
    public class CheerSubCommandUnitTest
    {
        [Fact]
        public async Task ExecuteAsync_ShouldRun_WhenCalledForTheFirstTime()
        {
            // Arrange
            var command = new CheerSubCommand();
            command.GetType().GetField("ultimoToca", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, DateTime.MinValue);

            // Act
            var response = await command.ExecuteAsync(null);

            // Assert
            Assert.Equal("!kappagen bracubiTrombeta ", response);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRun_WhenCalledAgainButMoreThanTwoMinutes()
        {
            // Arrange
            var command = new CheerSubCommand();
            var lastTimeRun = DateTime.UtcNow.AddMinutes(-2);
            command.GetType().GetField("ultimoToca", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, lastTimeRun);

            // Act
            var response = await command.ExecuteAsync(null);

            // Assert
            Assert.Equal("!kappagen bracubiTrombeta ", response);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenCalledInLessThan60Seconds()
        {
            // Arrange
            var command = new CheerSubCommand();
            var lastTimeRun = DateTime.UtcNow.AddSeconds(-61);
            command.GetType().GetField("ultimoToca", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, lastTimeRun);

            // Act
            var response = await command.ExecuteAsync(null);

            // Assert
            Assert.Contains("Aguarde mais", response);
            Assert.Contains("segundos para usar o comando !toca", response);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMessage_WhenCalledInMoreThan60Seconds()
        {
            // Arrange
            var command = new CheerSubCommand();
            var lastTimeRun = DateTime.UtcNow.AddSeconds(-1);
            command.GetType().GetField("ultimoToca", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, lastTimeRun);

            // Act
            var response = await command.ExecuteAsync(null);

            // Assert
            Assert.Contains("Aguarde mais", response);
            Assert.Contains("minuto e", response);
            Assert.Contains("segundo(s) para usar o comando !toca", response);
        }
    }
}
