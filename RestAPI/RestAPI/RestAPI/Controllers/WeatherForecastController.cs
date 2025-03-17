using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Service;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static ActivitySource source = new ActivitySource("RestAPI.WeatherForecase", "1.0.0");

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("WeatherForecast get");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("dosomework")]
        public async Task<string> DoSomeWork([FromQuery] string id, [FromQuery] string name)
        {
            _logger.LogInformation("DoSomeWork#");

            using (var activity = source.StartActivity("SomeWork"))
            {
                activity?.SetTag("id", id);
                activity?.SetTag("name", name);
                
                var forecasts = await StepOne();
                activity?.AddEvent(new ActivityEvent("Part way there"));
                
                await StepTwo();
                activity?.AddEvent(new ActivityEvent("Done now"));

                // Pretend something went wrong
                activity?.SetStatus(ActivityStatusCode.Ok, "everything done");
                        
                var summaries = forecasts.Select(i => i.Summary).ToList();
                var result = String.Join(",", summaries);

                return result;
            }
        }

        private async Task<List<WeatherForecast>> StepOne()
        {
            _logger.LogInformation("StepOne#");

            using (var activity = source.StartActivity("StepOne"))
            {
                await Task.Delay(1000);

                activity?.AddEvent(new ActivityEvent("Generate new weather forecast"));

                var forecasts = _weatherService.Get();

                return forecasts;
            }
        }


        private async Task StepTwo()
        {
            _logger.LogInformation("StepTwo#");

            using (var activity = source.StartActivity("StepTwo"))
            {
                await Task.Delay(1000);
            }
        }
    }
}
