using System;
using System.Net.Http;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;

// using MQTTnet.Client.Options;

namespace TeslaAPIHandler.Services
{
    public class TeslaApiWriter : ITeslaApiWriter
    {
        // for vehicle commands, which have to go through proxy
        private IMqttClient _mqttClient;
        private readonly string _host = "mqtt.beebotte.com";
        private readonly int _port = 1883;
        private readonly string _clientId = "makaizou";
        private readonly string _beebotte_token;

        // for vehicl endpoints, which can be directly access
        private readonly HttpClient _httpClient;
        private readonly string _authToken;
        private readonly string _vehicleTag;
        private readonly string _baseUrl = "https://fleet-api.prd.na.vn.cloud.tesla.com/api/1/vehicles";

        public TeslaApiWriter(string beebotte_token, string authToken, string vehicleTag)
        {
            _mqttClient = new MqttFactory().CreateMqttClient();
            _beebotte_token = beebotte_token;

            _httpClient = new HttpClient();
            _authToken = authToken;
            _vehicleTag = vehicleTag;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
        }

        public async Task InitializeAsync()
        {
            try
            {
                await ConnectAsync(_host, _port, _clientId, _beebotte_token, _beebotte_token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WRITE: Initialize] Error: {ex.Message}");
            }

        }
        public async Task<bool> WakeUpAsync()
        {
            string url = $"{_baseUrl}/{_vehicleTag}/wake_up";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"[WRITE: WakeUp] API call failed with status code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("response", out JsonElement responseElement) &&
                responseElement.TryGetProperty("state", out JsonElement state))
            {
                if (state.GetString() == "online")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            throw new Exception("[WRITE: WakeUp] Failed to retrieve data from response.");
        }
        public async Task ExecuteAsync(string command, object data = null)
        {
            string topic = "LC500/command";

            // オブジェクトをシリアライズ
            var payloadObject = new
            {
                command = command,
                data = data ?? new { }  // デフォルト値として空のオブジェクトを設定
            };
            string payload = JsonSerializer.Serialize(payloadObject);

            try
            {
                await PublishAsync(topic, payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WRITE] Error publishing message: {ex.Message}");
            }
        }
        private async Task ConnectAsync(string host, int port, string clientId, string username, string password)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .WithClientId(clientId)
                .WithCredentials(username, password)
                .Build();

            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("[WRITE] MQTT Disconnected. Reconnecting...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                await _mqttClient.ConnectAsync(options);
            };

            await _mqttClient.ConnectAsync(options);
            Console.WriteLine("[WRITE] Connected to MQTT Broker.");
        }

        private async Task PublishAsync(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                // .WithExactlyOnceQoS()
                .Build();

            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(message);
                Console.WriteLine($"[WRITE] Message published. topic: {topic}, payload: {payload}");
            }
            else
            {
                Console.WriteLine("[WRITE] MQTT Client is not connected. Unable to publish.");
            }
        }
    }
}