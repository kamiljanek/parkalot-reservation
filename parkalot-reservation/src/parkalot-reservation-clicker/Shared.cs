using System.Collections.Concurrent;
using Microsoft.Playwright;

namespace parkalot_reservation_clicker;

public static class Shared
{
    private static readonly ConcurrentQueue<string> _logQueue = new();
    private static readonly Task _logTask = Task.Run(ProcessLogQueue);
    
    public static void CaughtRequest(this IPage page)
    {
        page.Request += async (_, request) =>
        {
            if (request.Method == "POST")
            {
                var headers = request.Headers
                    .Select(h => $"'{h.Key}': '{h.Value}'")
                    .ToArray();

                var fetchCode = $@"
fetch('{request.Url}', {{
  method: '{request.Method}',
  headers: {{
    {string.Join(",\n    ", headers)}
  }},
  body: {GetRequestBody(request)}
}})
";
                
                _logQueue.Enqueue($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FETCH CODE]\n{fetchCode}\n\n");
            }
        };
    }

    private static async Task ProcessLogQueue()
    {
        while (true)
        {
            if (_logQueue.TryDequeue(out var logEntry))
            {
                await File.AppendAllTextAsync("requests.log", logEntry);
            }
            else
            {
                await Task.Delay(100);
            }
        }
    }
    
    public static async Task ClickNavBar(this IPage page)
    {
        var navBar = page.Locator("xpath=//*[@id='navbar']/div[1]").First;
        if (await navBar.IsEnabledAsync())
        {
            await navBar.ClickAsync();
        }
    }
    
    public static async Task<bool> IsClickedReserveButton(this IPage page)
    {
        var mainPageReserveButton = page.Locator("button:text('reserve')").First;

        if (!await mainPageReserveButton.IsVisibleAsync())
        {
            return false;
        }

        await mainPageReserveButton.ClickAsync(new LocatorClickOptions(){Timeout = 1000});
        return true;
    }
    
    
    private static string GetRequestBody(IRequest request)
    {
        if (request.Method == "POST")
        {
            var postData = request.PostData;
            return postData != null ? $"`{postData}`" : "null";
        }
        return "null";
    }
}