using System.Diagnostics;

namespace RestAPI.Service
{
    public class WeatherService : IWeatherService
    {
        private static ActivitySource source = new ActivitySource("RestAPI.WeatherForecase", "1.0.0");
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public List<WeatherForecast> Get()
        {
            using (var activity = source.StartActivity("WeatherServiceGet"))
            {
                var temp = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                });

                activity?.SetStatus(ActivityStatusCode.Ok, "weather done");

                return temp.ToList();
            }
        }
    }
}
