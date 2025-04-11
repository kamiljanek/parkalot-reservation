using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
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

        await DelayUntilFriday();
        
        using var client = new HttpClient();
        
        var accessToken = await GetToken(client, configSettings);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            configSettings.Bearer = accessToken;
        }

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configSettings.Bearer}");
        
        // await ReserveDesk(client, configSettings);
        // UNDONE: add counting days for ReserveDesk
        // UNDONE: add counting days for ReserveDesk


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

    static async Task ReserveDesk(HttpClient client, Configuration configSettings)
    {
        try
        {
            var url = new Uri(new Uri(configSettings.BaseUri), RelativeUriPath.DeskPath);

            var body = BuildDeskBody(configSettings);
            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine("Response: " + responseContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Request failed: " + ex.Message);
        }
    }

    static async Task SendRequest(HttpClient client, Configuration configSettings, string parkingSpot)
    {
        try
        {
            var url = new Uri(new Uri(configSettings.BaseUri), RelativeUriPath.ParkingPath);
            var body = BuildParkingBody(configSettings, parkingSpot);
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
    
    static async Task<string> GetToken(HttpClient client, Configuration configSettings)
    {
        var accessToken = "";
        try
        {
            var baseUrl = new Uri(new Uri(configSettings.RefreshTokenBaseUrl), RelativeUriPath.TokenPath);
            
            var queryParams = new Dictionary<string, string>
            {
                { "key", configSettings.Key }
            };

            var fullUrl = QueryHelpers.AddQueryString(baseUrl.OriginalString, queryParams);
            
            var requestBody = $"grant_type=refresh_token&refresh_token={configSettings.RefreshToken}";
            
            var content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                   
            var response = await client.PostAsync(fullUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();
        
            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(responseContent);
                if (doc.RootElement.TryGetProperty("access_token", out JsonElement tokenElement))
                {
                    accessToken = tokenElement.GetString();
                }
                else
                {
                    Console.WriteLine("Response does not contain an access token.");
                }
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
                Console.WriteLine(responseContent);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return accessToken ?? string.Empty;
    }
    
    static async Task DelayUntilFriday()
    {
        while (true)
        {
            var now = DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Friday && 
                now.TimeOfDay >= new TimeSpan(10, 0, 3) && 
                now.TimeOfDay <= new TimeSpan(10, 3, 0))
                return;

            Console.WriteLine($"{DateTime.Now:T} - Waiting for friday... 10:00:03");
            await Task.Delay(1000);
        }
    }

    private static object BuildParkingBody(Configuration? configSettings, string spotId)
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
    
    private static object BuildDeskBody(Configuration? configSettings)
    {
        var shiftValue = "00002400";
        var body = new
        {
            me = configSettings.MeId,
            parkingId = configSettings.DeskParkingId,
            uid = configSettings.MeId,
            spotId = "cccbbbbbaaaaa",
            day = 20171,
            // UNDONE: add counting days /\
            addShifts = new[] { shiftValue },
            removeShifts = new string[] { },
            v = configSettings.DeskBodyV
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
}