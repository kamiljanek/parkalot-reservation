using Microsoft.Playwright;

namespace parkalot_reservation;

public static class PlaywrightExtensions
{
    public static async Task ClickWhenReadyAsync(this IPage page, string selector, Func<Task> previousStep)
    {
        await page.WaitForLoadStateAsync();

        var locators = await page.Locator(selector).AllAsync();
        await page.Locator(selector).WaitForAsync(new LocatorWaitForOptions(){State = WaitForSelectorState.Visible, Timeout = 2000});

        if (locators.Count == 0 || await locators[0].IsDisabledAsync())
        {
            previousStep?.Invoke();
            await page.ClickWhenReadyAsync(selector, previousStep);
        }
        
        await locators[0].ClickAsync();
    }
}