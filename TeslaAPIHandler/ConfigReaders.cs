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
}
