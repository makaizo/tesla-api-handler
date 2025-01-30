using System;
using System.Threading.Tasks;
using System.Text.Json;
using UnityEngine;
using TeslaAPIHandler.Services;
using TeslaAPIHandler.Services.Mock;
using System.IO;

public class TeslaApiManager : MonoBehaviour
{
    private const string CONFIG_FILENAME = "credentials.json";

    [SerializeField]
    private static bool useMock = false;

    async void Start()
    {
        try
        {
            InitializeConfigFile();
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

    public async Task Main(string[] args)
    {
        Debug.Log("Start Main");

        // get vin and access_token
        string configFilePath = GetConfigFilePath();
        var config = ConfigHandler.ReadConfig(configFilePath);

        string accessToken;
        string refreshToken;

        IAuthTokenHandler authHandler = useMock ? new MockAuthTokenHandler() : new AuthTokenHandler();

        try
        {
            (accessToken, refreshToken) = await authHandler.RefreshTokenAsync(config.refreshToken, config.clientID);
            // Console.WriteLine($"New token: {accessToken}");
            // 新しいリフレッシュトークンをファイルに書き込む
            ConfigHandler.WriteConfig(configFilePath, refreshToken);
        }
        catch (Exception ex)
        {
            accessToken = "fallback_token"; // set a fake token when error occurred
            Console.WriteLine($"Error: {ex.Message}");

            // TODO: リフレッシュトークンが失効した場合の処理を追加;
        }

        // api reader client: unity -> tesla Fleet API -> vehicle
        ITeslaApiReader apiReader = useMock ? new MockTeslaApiReader() : new TeslaApiReader(accessToken, config.vin);
        // api writer client: unity -> MQTT broker(beebotte server) -> tesla vehicle Commands Proxy on RasPi -> tesla Fleet API -> vehicle
        ITeslaApiWriter apiWriter = useMock ? new MockTeslaApiWriter() : new TeslaApiWriter(config.beebotteToken, accessToken, config.vin);

        if (useMock)
        {
            Debug.LogWarning("You are using Mock!! Please set useMock to false.");
        }

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

            Debug.Log("Write API is not error occurred");

        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to API writer called: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }



    private static string GetStreamingAssetsPath()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            return "jar:file://" + Application.dataPath + "!/assets";
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer) // NOTE:Macは動作未確認
        {
            return Application.dataPath + "/Resources/Data/StreamingAssets";
        }
        else
        {
            return Application.streamingAssetsPath;
        }
    }

    private static string GetConfigFilePath()
    {
        return Path.Combine(Application.persistentDataPath, CONFIG_FILENAME);
    }

    private void InitializeConfigFile()
    {
        string persistentPath = GetConfigFilePath();
        Debug.Log($"persistentPath: {persistentPath}");

        if (!File.Exists(persistentPath))
        {
            string streamingPath = Path.Combine(GetStreamingAssetsPath(), CONFIG_FILENAME);
            Debug.Log($"streamingPath: {streamingPath}");
#if UNITY_EDITOR
            File.Copy(streamingPath, persistentPath);
#elif UNITY_ANDROID
            using (UnityWebRequest www = UnityWebRequest.Get(streamingPath))
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(persistentPath, www.downloadHandler.data);
                }
                else
                {
                    Debug.LogError($"Failed to download file: {www.error}");
                    throw new Exception($"Failed to download file: {www.error}");
                }
            }
#endif
            Debug.Log($"File copied is {File.Exists(persistentPath)})");
        }
    }
}
