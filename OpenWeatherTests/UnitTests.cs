using System;
using System.Threading.Tasks;
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
        public async Task OpenWeatherTest()
        {
            // Verify timestamp is within last minute
            DateTime verifyTimestamp = DateTime.UtcNow.AddMinutes(-10);

            Program.Run(null, new TestLogger());

            // Stream ID below must match single city defined in OpenWeatherQueries
            string streamId = $"OpenWeather_Current_San Leandro";

            // ADH does not validate OMF before sending a success response, so the test must check that the messages were successful
            using AuthenticationHandler adhAuthenticationHandler = new(Program.Settings.Resource, Program.Settings.ClientId, Program.Settings.ClientSecret);
            SdsService adhSdsService = new(Program.Settings.Resource, null, HttpCompressionMethod.GZip, adhAuthenticationHandler);
            ISdsDataService adhDataService = adhSdsService.GetDataService(Program.Settings.TenantId, Program.Settings.NamespaceId);
            CurrentWeather adhValue = await adhDataService.GetLastValueAsync<CurrentWeather>(streamId);
            Assert.True(adhValue.Timestamp > verifyTimestamp);
        }

        private sealed class TestLogger : ILogger
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
