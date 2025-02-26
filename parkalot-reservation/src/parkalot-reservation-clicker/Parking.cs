using Microsoft.Playwright;

namespace parkalot_reservation_clicker;

public static class Parking
{
    private static int _attempts = 0;

    public static async Task MakeReservation(IPage page)
    {
        await page.ClickNavBar();

        await page.ClickWhenReadyAsync("#animate span:text('Weekly Parking')", page.ClickNavBar);

        await Task.Delay(1000);
        
        while (true)
        {
            if (!await page.IsClickedReserveButton())
            {
                await Task.Delay(100);
                Console.WriteLine($"Parking attempt = {_attempts++}");
                continue;
            }
            break;
        }
        await ClickSelectAsync(page);
        
        await page.Locator("xpath=//*[@id='animate']/div/div/div[2]/div[3]/div/div[2]/div/div[5]").First.ClickAsync(); // choose fifth parking place
        
        page.CaughtRequest();
        
        await page.Locator("#animate button:text('reserve')").First.ClickAsync();
    }

    private static async Task ClickSelectAsync(IPage page)
    {
        try
        {
            await page.Locator("#animate div:text('select')").First.ClickAsync(new LocatorClickOptions(){Timeout = 500});
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await ClickSelectAsync2(page);
        }
    }
    
    private static async Task ClickSelectAsync2(IPage page)
    {
        try
        {
            await page.Locator("xpath=//*[@id='animate']/div/div/div[2]/div[3]/div/div/span/span").First.ClickAsync(new LocatorClickOptions(){Timeout = 500});
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await ClickSelectAsync(page);
        }
    }
}