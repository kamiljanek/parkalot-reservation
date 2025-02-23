using Microsoft.Extensions.Configuration;

namespace parkalot_reservation_request;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var configSettings = configuration.GetSection(Configuration.ConfigurationSectionName).Get<Configuration>();

        Console.WriteLine("App Name: " + configSettings.Bearer);
        Console.WriteLine("Version: " + configSettings.Url);
        
    }
}