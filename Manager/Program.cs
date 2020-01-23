using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace Manager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var token = await GetTokenAsync();
            Console.WriteLine(token);
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