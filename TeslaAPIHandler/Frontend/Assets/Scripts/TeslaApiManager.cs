using System;
using System.Threading.Tasks;
using System.Text.Json;
using UnityEngine;
using TeslaAPIHandler.Services;
using TeslaAPIHandler.Services.Mock;

public class TeslaApiManager : MonoBehaviour
{
    [SerializeField]
    private static bool useMock = true;

    async void Start()
    {
        try
        {
            await Main(new string[] { });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize Tesla API: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    void Update()
    {
    }

    public static async Task Main(string[] args)
    {
        Debug.Log("Start Main");

        // get vin and access_token
        string configFilePath = "credentials"; // without .json extension
        var config = ReadConfig(configFilePath);

        if (string.IsNullOrEmpty(config.refreshToken) || string.IsNullOrEmpty(config.vin) ||
            string.IsNullOrEmpty(config.beebotteToken) || string.IsNullOrEmpty(config.clientID))
        {
            Debug.LogError("Failed to read config file or missing required values");
            return;
        }

        Debug.Log("Config loaded");

        IAuthTokenHandler authHandler = useMock
            ? new MockAuthTokenHandler()
            : new AuthTokenHandler();
        var accessToken = await authHandler.RefreshTokenAsync(config.refreshToken, config.clientID);
        Debug.Log("New token loaded");

        // API クライアントの初期化
        ITeslaApiReader apiReader = useMock
            ? new MockTeslaApiReader()
            : new TeslaApiReader(accessToken, config.vin);
        Debug.Log("API reader initialized");

        ITeslaApiWriter apiWriter = useMock
            ? new MockTeslaApiWriter()
            : new TeslaApiWriter(config.beebotteToken, accessToken, config.vin);
        Debug.Log("API writer initialized");

        try
        {

            int? batteryLevel = await apiReader.GetBatteryLevelAsync();
            Debug.Log($"Battery Level: {batteryLevel}");

            string? location = await apiReader.GetLocationAsync();
            Debug.Log($"Location: {location}");

            int? speed = await apiReader.GetSpeedAsync();
            Debug.Log($"Speed: {speed}");


        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to API reader called: {ex.Message}\nStack trace: {ex.StackTrace}");
        }

        try
        {
            await apiWriter.InitializeAsync();

            // WakeUpは実際に実行されるのでデバッグ時に何度も実行しないよう注意
            // bool result = await apiWriter.WakeUpAsync();
            // Console.WriteLine($"WakeUp: {result}");

            // 他のコマンドはラズパイのサーバ起動後に、実行されるため、デバッグ時に何度も実行しても問題ない
            // await apiWriter.ExecuteAsync(command: "actuate_trunk", data: new { which_trunk = "front" });
            await apiWriter.ExecuteAsync(command: "door_lock");
            // await apiWriter.ExecuteAsync(command: "door_unlock");
            // await apiWriter.ExecuteAsync(command: "flash_lights");
            // await apiWriter.ExecuteAsync(command: "window_control", data: new { lat = 45, lon = 45, command = "close" });

            // // remote_boombox
            // //  Sound IDs:
            // //      0:random fart
            // //      2000: locate ping
            // await apiWriter.ExecuteAsync(command: "remote_boombox", data: new { sound = 0 });
            // await apiWriter.ExecuteAsync(command: "auto_conditioning_start");
            // await apiWriter.ExecuteAsync(command: "auto_conditioning_stop");
            // await apiWriter.ExecuteAsync(command: "media_toggle_playback");
            // await apiWriter.ExecuteAsync(command: "adjust_volume", data: new { volume = 5 });
            // await apiWriter.ExecuteAsync(command: "set_bioweapon_mode", data: new { on = true, manual_override = true });

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to API writer called: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    private static (string refreshToken, string vin, string beebotteToken, string clientID) ReadConfig(string fileName)
    {
        try
        {
            // Load JSON file from Resources folder
            TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
            using var doc = JsonDocument.Parse(jsonFile.text);
            JsonElement root = doc.RootElement;

            string refreshToken = root.GetProperty("refreshToken").GetString();
            string vin = root.GetProperty("vin").GetString();
            string beebotteToken = root.GetProperty("beebotteToken").GetString();
            string clientID = root.GetProperty("clientID").GetString();

            return (refreshToken, vin, beebotteToken, clientID);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading config file: {ex.Message}");
            throw;
        }
    }
}
