namespace Manager
{
    public static class Settings
    {
        public const string AzureAD = "https://login.microsoftonline.com/";
        public const string ClientId = "227c942d-f240-4c36-b395-9ca11e2cd1c9";
        public const string TenantId = "63368336-e789-4b48-a0ba-7c956298e199";
        public const string Authority = AzureAD + TenantId;
        public const string DigitalTwinsResourceId = "0b07f429-9f4b-4714-9392-cc5e8e80c8b0";
    }
}
