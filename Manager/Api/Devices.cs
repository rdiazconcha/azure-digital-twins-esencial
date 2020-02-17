using Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Manager.Api
{
    class Devices
    {
        public static async Task<Guid> GetOrCreateDeviceAsync(string deviceName, string hardwareId,
            Guid spaceId, HttpClient httpClient)
        {
            var devices = await SearchDeviceAsync(deviceName, httpClient);
            if (devices.Any())
            {
                return devices.First().Id;
            }
            var newId = await CreateDeviceAsync(deviceName, hardwareId, spaceId, httpClient);
            return newId;
        }

        public static async Task<IEnumerable<DeviceQuery>> SearchDeviceAsync(string deviceName,
            HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"devices?$filter=Name eq '{deviceName}'");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var devices = JsonSerializer.Deserialize<IEnumerable<DeviceQuery>>(content,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return devices;
            }
            return null;
        } 
        public static async Task<Guid> CreateDeviceAsync(string name, 
            string hardwareId, Guid spaceId, HttpClient httpClient)
        {
            var newDevice = new { name, hardwareId, spaceId };
            var content = JsonSerializer.Serialize(newDevice);
            var response = await httpClient.PostAsync("devices",
                new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var newId = JsonSerializer.Deserialize<Guid>(result);
                return newId;
            }
            return Guid.Empty;
        }

        public static async Task<DeviceQuery> GetDeviceAsync(Guid id, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"devices/{id}?includes=connectionString");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DeviceQuery>(content, 
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            return null;
        }
    }
}
