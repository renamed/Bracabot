using Bracabot2.Domain.Extensions;
using System;
using Xunit;

namespace Bracabot.UnitTests.Support
{
    public class TimespanExtensionsUnitTest
    {
        [Fact]
        public void GetReadable_ShouldReturn0Seconds_WhenSecondIsZero()
        {
            var ts = TimeSpan.FromSeconds(0);

            var result = ts.GetReadable();

            Assert.Equal("0 segundo", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnWeekInSingular_WhenItsACompleteWeek()
        {
            var ts = TimeSpan.FromDays(7);

            var result = ts.GetReadable();

            Assert.Equal("1 semana", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnWeekInPlural_WhenItsACompleteWeek()
        {
            var ts = TimeSpan.FromDays(14);

            var result = ts.GetReadable();

            Assert.Equal("2 semanas", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnDayInSingular_WhenItsACompleteDay()
        {
            var ts = TimeSpan.FromDays(1);

            var result = ts.GetReadable();

            Assert.Equal("1 dia", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnDayInPlural_WhenItsACompleteDay()
        {
            var ts = TimeSpan.FromDays(4);

            var result = ts.GetReadable();

            Assert.Equal("4 dias", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnHourInSingular_WhenItsACompleteHour()
        {
            var ts = TimeSpan.FromHours(1);

            var result = ts.GetReadable();

            Assert.Equal("1 hora", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnHourInPlural_WhenItsACompleteHour()
        {
            var ts = TimeSpan.FromHours(15);

            var result = ts.GetReadable();

            Assert.Equal("15 horas", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnMinuteInSingular_WhenItsACompleteMinute()
        {
            var ts = TimeSpan.FromMinutes(1);

            var result = ts.GetReadable();

            Assert.Equal("1 minuto", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnMinuteInPlural_WhenItsACompleteMinute()
        {
            var ts = TimeSpan.FromMinutes(55);

            var result = ts.GetReadable();

            Assert.Equal("55 minutos", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnSecondInSingular_WhenItsACompleteSecond()
        {
            var ts = TimeSpan.FromSeconds(1);

            var result = ts.GetReadable();

            Assert.Equal("1 segundo", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnSecondInPlural_WhenItsACompleteSecond()
        {
            var ts = TimeSpan.FromSeconds(49);

            var result = ts.GetReadable();

            Assert.Equal("49 segundos", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnMinutesAndSeconds_WhenSecondIsOver60()
        {
            var ts = TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(33));

            var result = ts.GetReadable();

            Assert.Equal("4 minutos, 33 segundos", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnHoursMinutesAndSeconds_WhenMinuteIsOver60()
        {
            var ts = TimeSpan.FromHours(1)
                        .Add(TimeSpan.FromMinutes(13))
                        .Add(TimeSpan.FromSeconds(33));

            var result = ts.GetReadable();

            Assert.Equal("1 hora, 13 minutos, 33 segundos", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnDaysHoursMinutesAndSeconds_WhenHourIsOver24()
        {
            var ts = TimeSpan.FromDays(5)
                        .Add(TimeSpan.FromHours(2))
                        .Add(TimeSpan.FromMinutes(13))
                        .Add(TimeSpan.FromSeconds(33));

            var result = ts.GetReadable();

            Assert.Equal("5 dias, 2 horas, 13 minutos, 33 segundos", result);
        }

        [Fact]
        public void GetReadable_ShouldReturnWeeksDaysHoursMinutesAndSeconds_WhenDaysIsOver7()
        {
            var ts = TimeSpan.FromDays(20)
                        .Add(TimeSpan.FromDays(5))
                        .Add(TimeSpan.FromHours(2))
                        .Add(TimeSpan.FromMinutes(13))
                        .Add(TimeSpan.FromSeconds(33));

            var result = ts.GetReadable();

            Assert.Equal("3 semanas, 4 dias, 2 horas, 13 minutos, 33 segundos", result);
        }
    }
}
