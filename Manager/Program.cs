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
            Console.WriteLine(spaceId);
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