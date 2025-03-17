using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
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
                await StepOne();
                activity?.AddEvent(new ActivityEvent("Part way there"));
                await StepTwo();
                activity?.AddEvent(new ActivityEvent("Done now"));

                // Pretend something went wrong
                activity?.SetStatus(ActivityStatusCode.Ok, "everything done");
            }

            return "123";
        }

        private async Task StepOne()
        {
            _logger.LogInformation("StepOne#");

            using (var activity = source.StartActivity("StepOne"))
            {
                await Task.Delay(1000);
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
