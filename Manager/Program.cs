using Manager.Api;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Manager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var token = await GetNewOrExistingTokenAsync();
            var httpClient = GetHttpClient(token);

            var spaceId = await Spaces.GetOrCreateSpaceAsync("Habitación", "Room", httpClient);
            var resourceId = await Resources.GetOrCreateResourceAsync(spaceId, "IotHub", httpClient);

            bool isProvisioning = true;
            while (isProvisioning)
            {
                var resource = await Resources.GetResourceAsync(resourceId, httpClient);
                if (resource.Status.ToLower() == "provisioning")
                {
                    await Task.Delay(2000);
                }
                else
                {
                    isProvisioning = false;
                }
            }

            var deviceId = await Devices.GetOrCreateDeviceAsync("Dispositivo 1", "ABC123", 
                spaceId, httpClient);
            var device = await Devices.GetDeviceAsync(deviceId, httpClient);
            var sensorId = await Sensors.GetOrCreateSensorAsync("Temperatura", "TEMP123", deviceId,
                httpClient);
            var matcherId = await Matchers.GetOrCreateMatcherAsync("Matcher 1", spaceId, "Temperatura",
                httpClient);

            Console.WriteLine($"SpaceId {spaceId}");
            Console.WriteLine($"ResourceId {resourceId}");
            Console.WriteLine($"DeviceId {deviceId}");
            Console.WriteLine($"ConnectionString {device.ConnectionString}");
            Console.WriteLine($"SensorId {sensorId}");
            Console.WriteLine($"MatcherId {matcherId}");
        }

        private static HttpClient GetHttpClient(string token)
        {
            var baseUrl = @"https://digital-twins-curso.westus2.azuresmartspaces.net/management/api/v1.0/";
            var httpClient = new HttpClient() { BaseAddress = new Uri(baseUrl) };
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return httpClient;
        }

        private static async Task<string> GetNewOrExistingTokenAsync()
        {
            string fileName = "token";
            string token = null;
            if (File.Exists(fileName))
            {
                using var streamReader = new StreamReader(fileName);
                token = await streamReader.ReadToEndAsync();
            }
            else
            {
                token = await GetTokenAsync();
                using var streamWriter = new StreamWriter(fileName);
                await streamWriter.WriteAsync(token);
            }
            return token;
        }

        static async Task<string> GetTokenAsync()
        {
            var authContext = new AuthenticationContext(Settings.Authority);
            var deviceCodeResult = await
                authContext.AcquireDeviceCodeAsync(Settings.DigitalTwinsResourceId, Settings.ClientId);
            Console.WriteLine(deviceCodeResult.Message);
            var token = await authContext.AcquireTokenByDeviceCodeAsync(deviceCodeResult);
            return token.AccessToken;
        }
    }
}