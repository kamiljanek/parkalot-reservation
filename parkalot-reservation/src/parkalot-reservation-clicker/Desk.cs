using Microsoft.Playwright;

namespace parkalot_reservation;

public static class Desk
{
    public static async Task MakeReservation(IPage page)
    {
        await page.ClickNavBar();

        await page.ClickWhenReadyAsync("#animate span:text('QIAGEN Wrocław')", page.ClickNavBar);

        await Task.Delay(1000);

        await page.Keyboard.PressAsync("End");

        await Task.Delay(1000);

        var reserveButtons = await page.Locator("button:text('reserve')").AllAsync();

        foreach (var reserveButton in reserveButtons)
        {
            await page.IsClickedReserveButton();

            await page.Locator("#animate div:text('select')").First.ClickAsync();
        
            await page.Locator("#animate div:text('RED Q2.2.08.67')").First.ClickAsync();
        
            page.CaughtRequest();
        
            await page.Locator("#animate button:text('reserve')").First.ClickAsync();
            
            await Task.Delay(3000);
        }
    }
}