using System;
using Microsoft.Extensions.Logging;
using OpenWeather;
using OSIsoft.Data;
using OSIsoft.Data.Http;
using OSIsoft.Identity;
using Xunit;

namespace OpenWeatherTests
{
    public class UnitTests
    {
        [Fact]
        public void OpenWeatherTest()
        {
            // Verify timestamp is within last minute
            DateTime verifyTimestamp = DateTime.UtcNow.AddMinutes(-10);

            Program.Run(null, new TestLogger());

            // Stream ID below must match single city defined in OpenWeatherQueries
            string streamId = $"OpenWeather_Current_San Leandro";

            // ADH does not validate OMF before sending a success response, so the test must check that the messages were successful
            using AuthenticationHandler adhAuthenticationHandler = new(Program.Settings.AdhUri, Program.Settings.AdhClientId, Program.Settings.AdhClientSecret);
            SdsService adhSdsService = new(Program.Settings.AdhUri, null, HttpCompressionMethod.GZip, adhAuthenticationHandler);
            ISdsDataService adhDataService = adhSdsService.GetDataService(Program.Settings.AdhTenantId, Program.Settings.AdhNamespaceId);
            CurrentWeather adhValue = adhDataService.GetLastValueAsync<CurrentWeather>(streamId).Result;
            Assert.True(adhValue.Timestamp > verifyTimestamp);
        }

        private class TestLogger : ILogger
        {
            public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception ex, Func<TState, Exception, string> func)
            {
                Console.WriteLine(state);
            }

            public bool IsEnabled(LogLevel level)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return state as IDisposable;
            }
        }
    }
}
