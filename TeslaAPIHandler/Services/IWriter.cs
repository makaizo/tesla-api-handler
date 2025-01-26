namespace TeslaAPIHandler.Services
{
    public interface ITeslaApiWriter
    {
        Task InitializeAsync();
        Task<bool> WakeUpAsync();
        Task ExecuteAsync(string command, object? data = null);

    }
}