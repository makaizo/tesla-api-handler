using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace TeslaAPIHandler.Services
{
    public class TeslaApiReader : ITeslaApiReader
    {
        private readonly HttpClient _httpClient;
        private readonly string _authToken;
        private readonly string _vehicleTag;
        private readonly string _baseUrl = "https://fleet-api.prd.na.vn.cloud.tesla.com/api/1/vehicles";

        public TeslaApiReader(string authToken, string vehicleTag)
        {
            _httpClient = new HttpClient();
            _authToken = authToken;
            _vehicleTag = vehicleTag;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
        }

        public async Task<int?> GetBatteryLevelAsync()
        {
            string url = $"{_baseUrl}/{_vehicleTag}/vehicle_data";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"[READ] API call failed with status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("response", out JsonElement responseElement) &&
                responseElement.TryGetProperty("charge_state", out JsonElement chargeState) &&
                chargeState.TryGetProperty("battery_level", out JsonElement batteryLevel))
            {
                return batteryLevel.ValueKind == JsonValueKind.Null ? null : batteryLevel.GetInt32();
            }

            throw new Exception("[READ] Failed to retrieve data from response.");
        }

        public async Task<string?> GetLocationAsync()
        {
            string url = $"{_baseUrl}/{_vehicleTag}/vehicle_data?endpoints=location_data";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"[READ] API call failed with status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("response", out JsonElement responseElement) &&
                responseElement.TryGetProperty("drive_state", out JsonElement locationData))
            {
                return locationData.ValueKind == JsonValueKind.Null ? null : locationData.GetRawText();
            }

            throw new Exception("[READ] Failed to retrieve data from response.");
        }

        public async Task<int?> GetSpeedAsync()
        {
            string url = $"{_baseUrl}/{_vehicleTag}/vehicle_data";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"[READ] API call failed with status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("response", out JsonElement responseElement) &&
                responseElement.TryGetProperty("drive_state", out JsonElement driveState) &&
                driveState.TryGetProperty("speed", out JsonElement speed))
            {
                return speed.ValueKind == JsonValueKind.Null ? null : speed.GetInt32();
            }

            throw new Exception("[READ] Failed to retrieve data from response.");
        }
    }
}