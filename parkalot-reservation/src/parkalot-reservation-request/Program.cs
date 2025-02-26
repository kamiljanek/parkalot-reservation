using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace parkalot_reservation_request;

static class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var configSettings = configuration.GetSection(Configuration.ConfigurationSectionName).Get<Configuration>();

        using var client = new HttpClient();

        client.AddHeaders(configSettings);

        // UNDONE: how to update bearer token automatically

        await DelayUntilFriday();

        var parkingSpots = ParkingSpots.GetSelectedSpots();

        for (var i = 0; i < 20; i++)
        {
            foreach (var parkingSpot in parkingSpots)
            {
                Task.Run(async () => await SendRequest(client, configSettings, parkingSpot));

                Console.WriteLine($"Task.Run for '{parkingSpot}' sent : {DateTime.Now:O}");
                await Task.Delay(300);
            }
        }

        await Task.Delay(-1);
    }

    static async Task SendRequest(HttpClient client, Configuration configSettings, string parkingSpot)
    {
        try
        {
            var url = new Uri(new Uri(configSettings.BaseUri), RelativeUriPath.ParkingPath);
            var body = BuildBody(configSettings, parkingSpot);
            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var message = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response status of spotId '{parkingSpot}' : {response.StatusCode} / {message} : {DateTime.Now:O}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    static async Task DelayUntilFriday()
    {
        while (true)
        {
            var now = DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Friday && 
                now.TimeOfDay >= new TimeSpan(10, 0, 10) && 
                now.TimeOfDay <= new TimeSpan(10, 3, 0))
                return;

            Console.WriteLine($"{DateTime.Now:T} - Waiting for friday... 10:00:10");
            await Task.Delay(1000);
        }
    }

    private static object BuildBody(Configuration? configSettings, string spotId)
    {
        var shiftValue = "00002400";
        var body = new
        {
            me = configSettings.MeId,
            parkingId = configSettings.ParkingParkingId,
            uid = configSettings.MeId,
            resources = BuildResources(shiftValue, spotId)
        };
        return body;
    }

    private static object BuildResources(string shiftValue, string spotId)
    {
        var resources = new List<object>();
        
        var startDate = new DateTime(1970, 1, 1);
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
        DateTime nearestMonday = DateTime.Today.AddDays(daysUntilMonday == 0 ? 7 : daysUntilMonday);
        var mondayValue = (nearestMonday - startDate).TotalDays;

        for (int day = 0; day < 5; day++)
        {
            resources.Add(new { day = mondayValue + day, spotId = spotId, addShifts = new[] { shiftValue }, removeShifts = Array.Empty<string>() });
        }
        
        return resources.ToArray();
    }

    private static void AddHeaders(this HttpClient client, Configuration? configSettings)
    {
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configSettings.Bearer}");
        client.DefaultRequestHeaders.Add("Priority", "u=1, i");
        client.DefaultRequestHeaders.Add("Sec-CH-UA", "\"Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"");
        client.DefaultRequestHeaders.Add("Sec-CH-UA-Mobile", "?0");
        client.DefaultRequestHeaders.Add("Sec-CH-UA-Platform", "\"Windows\"");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
    }
}