using System.Threading.Tasks;

namespace TeslaAPIHandler.Services.Mock
{
    public class MockTeslaApiReader : ITeslaApiReader
    {
        public Task<int?> GetBatteryLevelAsync()
        {
            return Task.FromResult<int?>(85);
        }

        public Task<string?> GetLocationAsync()
        {
            return Task.FromResult<string?>("{\"latitude\":35.6812,\"longitude\":139.7671}");
        }

        public Task<int?> GetSpeedAsync()
        {
            return Task.FromResult<int?>(60);
        }
    }
}
