using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

public class TeslaApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _authToken;
    private readonly string _baseUrl = "https://fleet-api.prd.na.vn.cloud.tesla.com/api/1/vehicles";

    public TeslaApiClient(string authToken)
    {
        _httpClient = new HttpClient();
        _authToken = authToken;
    }

    public async Task<int?> GetBatteryLevelAsync(string vehicleTag)
    {
        string url = $"{_baseUrl}/{vehicleTag}/vehicle_data";

        // Authorizationヘッダーを設定
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API call failed with status code: {response.StatusCode}");
        }

        string json = await response.Content.ReadAsStringAsync();

        // JSONレスポンスをパース
        using var doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("response", out JsonElement responseElement) &&
            responseElement.TryGetProperty("charge_state", out JsonElement chargeState) &&
            chargeState.TryGetProperty("battery_level", out JsonElement batteryLevel))
        {
            if (batteryLevel.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return batteryLevel.GetInt32();
        }

        throw new Exception("Failed to retrieve data from response.");
    }
    public async Task<String?> GetLocationAsync(string vehicleTag)
    {
        string url = $"{_baseUrl}/{vehicleTag}/vehicle_data?endpoints=location_data";

        // Authorizationヘッダーを設定
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API call failed with status code: {response.StatusCode}");
        }

        string json = await response.Content.ReadAsStringAsync();

        // JSONレスポンスをパース
        using var doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("response", out JsonElement responseElement) &&
        responseElement.TryGetProperty("drive_state", out JsonElement locationData))
        {
            if (locationData.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return locationData.GetRawText();
        }

        throw new Exception("Failed to retrieve data from response.");
    }
    public async Task<int?> GetSpeedAsync(string vehicleTag)
    {
        string url = $"{_baseUrl}/{vehicleTag}/vehicle_data";

        // Authorizationヘッダーを設定
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"API call failed with status code: {response.StatusCode}");
        }

        string json = await response.Content.ReadAsStringAsync();

        // JSONレスポンスをパース
        using var doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("response", out JsonElement responseElement) &&
        responseElement.TryGetProperty("drive_state", out JsonElement driveState) &&
        driveState.TryGetProperty("speed", out JsonElement speed))
        {

            if (speed.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return speed.GetInt32();
        }

        throw new Exception("Failed to retrieve data from response.");
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine($"Start Main");
        string configFilePath = "credentials.json";  // config.json ファイルのパス
        var config = ReadConfig(configFilePath);

        string authToken = config.authToken;
        string vehicleTag = config.vin;

        var client = new TeslaApiClient(authToken);

        try
        {
            int? batteryLevel = await client.GetBatteryLevelAsync(vehicleTag);
            Console.WriteLine($"Battery Level: {batteryLevel}%");
            String? location_data = await client.GetLocationAsync(vehicleTag);
            Console.WriteLine($"Location: {location_data}");
            int? speed = await client.GetSpeedAsync(vehicleTag);
            Console.WriteLine($"Speed: {speed}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // config.jsonを読み込んで、必要な情報を返すメソッド
    private static (string authToken, string vin) ReadConfig(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            string authToken = root.GetProperty("authToken").GetString();
            string vin = root.GetProperty("vin").GetString();

            return (authToken, vin);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading config file: {ex.Message}");
            throw;
        }
    }
}


// string url = "https://umayadia-apisample.azurewebsites.net/api/persons/Shakespeare";

// using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
// {
//     using (System.Net.Http.HttpResponseMessage response = client.GetAsync(url).Result)
//     {
//         string responseBody = response.Content.ReadAsStringAsync().Result;
//         System.Diagnostics.Debug.WriteLine(responseBody);
//     }
// }