using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazorAspireApp.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public record GetWeatherForecastsQuery : IRequest<IEnumerable<WeatherForecast>>;

public class GetWeatherForecastsQueryHandler : IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQuery request, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var rng = new Random();

        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorAspireApp/v1.0 (dvddo1970@yahoo.com)");
        HttpResponseMessage resp = await httpClient.GetAsync("https://api.weather.gov/gridpoints/SGX/43,56/forecast");
        
        WeatherForecastResponse wres = await resp.Content.ReadFromJsonAsync<WeatherForecastResponse>();

        return wres.properties.periods.Select(period => new WeatherForecast
        {
            Date = period.startTime,
            TemperatureC = period.temperature,
            Summary = $"{period.name}: {period.shortForecast}"
        });
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Elevation
{
    public string unitCode { get; set; }
    public double value { get; set; }
}

public class Geometry
{
    public string type { get; set; }
    public List<List<List<double>>> coordinates { get; set; }
}

public class Period
{
    public int number { get; set; }
    public string name { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }
    public bool isDaytime { get; set; }
    public int temperature { get; set; }
    public string temperatureUnit { get; set; }
    public string temperatureTrend { get; set; }
    public ProbabilityOfPrecipitation probabilityOfPrecipitation { get; set; }
    public string windSpeed { get; set; }
    public string windDirection { get; set; }
    public string icon { get; set; }
    public string shortForecast { get; set; }
    public string detailedForecast { get; set; }
}

public class ProbabilityOfPrecipitation
{
    public string unitCode { get; set; }
    public int value { get; set; }
}

public class Properties
{
    public string units { get; set; }
    public string forecastGenerator { get; set; }
    public DateTime generatedAt { get; set; }
    public DateTime updateTime { get; set; }
    public string validTimes { get; set; }
    public Elevation elevation { get; set; }
    public List<Period> periods { get; set; }
}

public class WeatherForecastResponse
{
    [JsonProperty("@context")]
    public List<object> context { get; set; }
    public string type { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }
}

