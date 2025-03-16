using System.Threading.Tasks;

namespace TeslaAPIHandler.Services.Mock
{
    public class MockTeslaApiReader : ITeslaApiReader
    {
        public Task<int?> GetBatteryLevelAsync()
        {
            return Task.FromResult<int?>(12);
        }

        public Task<string?> GetLocationAsync()
        {
            return Task.FromResult<string?>("{\"latitude\":12.3456,\"longitude\":789.1234}");
        }

        public Task<int?> GetSpeedAsync()
        {
            return Task.FromResult<int?>(34);
        }
    }
}
