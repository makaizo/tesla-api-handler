using System;
using System.Threading.Tasks;

namespace TeslaAPIHandler.Services.Mock
{
    public class MockTeslaApiWriter : ITeslaApiWriter
    {
        public Task InitializeAsync()
        {
            Console.WriteLine("[MOCK] Initialize called");
            return Task.CompletedTask;
        }

        public Task<bool> WakeUpAsync()
        {
            Console.WriteLine("[MOCK] WakeUp called");
            return Task.FromResult(true);
        }

        public Task ExecuteAsync(string command, object? data = null)
        {
            Console.WriteLine($"[MOCK] Execute called with command: {command}, data: {data}");
            return Task.CompletedTask;
        }

        private async Task ConnectAsync(string host, int port, string clientId, string username, string password)
        {
            Console.WriteLine($"[MOCK] MQTT Connect called with host:{host}, port:{port}, clientId:{clientId}");
            await Task.CompletedTask;
        }

        private async Task PublishAsync(string topic, string payload)
        {
            Console.WriteLine($"[MOCK] MQTT Publish called with topic:{topic}, payload:{payload}");
            await Task.CompletedTask;
        }
    }
}
