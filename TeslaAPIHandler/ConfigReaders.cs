using System;
using System.IO;
using System.Text.Json;

public static class ConfigReader
{
    public static (string refreshToken, string vin, string beebotteToken, string clientID) ReadConfig(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
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
    public static void WriteConfig(string filePath, string newRefreshToken)
    {
        try
        {
            // 既存の設定を読み込む（refreshToken以外はそのまま保持）
            var (oldRefreshToken, vin, beebotteToken, clientID) = ReadConfig(filePath);

            // 更新するデータ
            var updatedConfig = new
            {
                refreshToken = newRefreshToken,  // 更新
                vin = vin,  // そのまま
                beebotteToken = beebotteToken,  // そのまま
                clientID = clientID  // そのまま
            };

            string json = JsonSerializer.Serialize(updatedConfig, new JsonSerializerOptions { WriteIndented = true });

            // JSONをファイルに書き込む
            File.WriteAllText(filePath, json);

            Console.WriteLine("Config file updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing config file: {ex.Message}");
        }
    }
}
