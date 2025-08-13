using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using static BlazorAspireApp.Web.AuthorizationApiClient;

namespace BlazorAspireApp.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecasts[]> GetWeatherAsync(IJSRuntime jSRuntime, int maxItems = 10, CancellationToken cancellationToken = default)
    {
        httpClient = await SetHeaderToken(jSRuntime, httpClient);

        List<WeatherForecasts>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecasts>("/api/WeatherForecasts", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}

public record WeatherForecasts(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
