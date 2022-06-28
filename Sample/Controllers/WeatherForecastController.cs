using Microsoft.AspNetCore.Mvc;
using PostHog;
using PostHog.Model;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IPostHogClient _postHogClient;

        public WeatherForecastController(IPostHogClient postHogClient)
        {
            _postHogClient = postHogClient;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var properties = new Properties()
                .SetEventProperty("event", "value")
                .SetUserProperty("user-property-to-set", "value") // $set equivalent
                .SetUserPopertyOnce("user-property-to-set-once", "value"); // $set_once equivalent

            _postHogClient.Capture("a86818cc-c84e-4453-9c48-d7bb636e7f2d", "Fetch weather forecast", properties);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}