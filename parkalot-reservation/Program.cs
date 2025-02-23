using Microsoft.Playwright;

namespace parkalot_reservation;

class Program
{
    // First run: chrome.exe --remote-debugging-port=9222 --user-data-dir="C:\chrome-profile"

    static async Task Main()
    {
        var reservationType = EReservationType.Desk;
        
        
        using var playwright = await Playwright.CreateAsync();

        string userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "playwright-profile");

        var browser = await playwright.Chromium.LaunchPersistentContextAsync(userDataPath, new BrowserTypeLaunchPersistentContextOptions
        {
            Channel = "chrome",
            Headless = false
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://app.parkalot.io/#/client");
        await Task.Delay(2000);


        switch (reservationType)
        {
            case EReservationType.Desk:
                await Desk.MakeReservation(page);
                break;
            case EReservationType.Parking:
                await Parking.MakeReservation(page);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        await Task.Delay(-1);
        await browser.CloseAsync();
    }
}