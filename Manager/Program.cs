using Manager.Api;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Linq;
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

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Especifique [Aprovisionar] o [Monitorear]");
                return;
            }

            if (args?[0].ToLower() == "aprovisionar")
            {
                await ProvisionAsync(httpClient);
            }
            else
            {
                await MonitorAsync(httpClient);
            }
        }

        private static async Task MonitorAsync(HttpClient httpClient)
        {
            for (int i = 0; i < 60; i++)
            {
                var spaces = await Spaces.GetAllSpacesAsync(httpClient, true);
                var spacesData = spaces.Where(s => s.Values != null && 
                s.Values.Any(v => v.Type == "condicion"));
                if (spacesData.Any())
                {
                    foreach (var item in spacesData)
                    {
                        foreach (var value in item.Values)
                        {
                            Console.WriteLine($"Timestamp: {value.Timestamp}\nValue: {value.Value}\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No es posible encontrar habitaciones");
                }

                await Task.Delay(2000);
            }
        }
            

        private static async Task ProvisionAsync(HttpClient httpClient)
        {
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
            var sensorId = await Sensors.GetOrCreateSensorAsync("Temperature", "TEMP2000", deviceId,
                httpClient);
            var matcherId = await Matchers.GetOrCreateMatcherAsync("Matcher 1", spaceId, "Temperature",
                httpClient);
            var script = new StreamReader(@"js\udf.js").ReadToEnd();
            var udfId = await UserDefinedFunctions.GetOrCreateUserDefinedFunctionAsync("Temperature-Udf-1",
                spaceId, new[] { matcherId },
                script,
                httpClient);
            var roleAssignmentId = await RoleAssignments.CreateRoleAssignmentAsync(udfId,
                spaceId, httpClient);

            Console.WriteLine($"SpaceId {spaceId}");
            Console.WriteLine($"ResourceId {resourceId}");
            Console.WriteLine($"DeviceId {deviceId}");
            Console.WriteLine($"ConnectionString {device.ConnectionString}");
            Console.WriteLine($"SensorId {sensorId}");
            Console.WriteLine($"MatcherId {matcherId}");
            Console.WriteLine($"UdfId {udfId}");
            Console.WriteLine($"RoleAssignmentId {roleAssignmentId}");
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