using System;
using System.Dynamic;
using System.Threading.Tasks;
using TeslaAPIHandler;
using TeslaAPIHandler.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Start Main");

        // get vin and access_token
        string configFilePath = "credentials.json";
        var config = ConfigReader.ReadConfig(configFilePath);

        IAuthTokenHandler authHandler = new AuthTokenHandler();
        var accessToken = await authHandler.RefreshTokenAsync(config.refreshToken, config.clientID);
        // Console.WriteLine($"New token: {accessToken}");

        // api reader client: unity -> tesla Fleet API -> vehicle
        ITeslaApiReader apiReader = new TeslaApiReader(accessToken, config.vin);
        // api writer client: unity -> MQTT broker(beebotte server) -> tesla vehicle Commands Proxy on RasPi -> tesla Fleet API -> vehicle
        ITeslaApiWriter apiWriter = new TeslaApiWriter(config.beebotteToken, accessToken, config.vin);

        try
        {

            int? batteryLevel = await apiReader.GetBatteryLevelAsync();
            Console.WriteLine($"Battery Level: {batteryLevel}");

            string? location = await apiReader.GetLocationAsync();
            Console.WriteLine($"Location: {location}");

            int? speed = await apiReader.GetSpeedAsync();
            Console.WriteLine($"Speed: {speed}");


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
