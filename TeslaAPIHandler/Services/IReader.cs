using System.Threading.Tasks;

namespace TeslaAPIHandler.Services
{
    public interface ITeslaApiReader
    {
        Task<int?> GetBatteryLevelAsync();
        Task<string?> GetLocationAsync();
        Task<int?> GetSpeedAsync();
    }
}