using Microsoft.AspNetCore.Mvc;
using RestAPI.Model;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SineWaveController : Controller
    {
        private readonly ILogger<SineWaveController> _logger;

        public SineWaveController(ILogger<SineWaveController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetSinewaveData")]
        public IEnumerable<SineWaveData> Get(double pad, double amplitude, double phase, int pointCount, double freq)
        {
            List<SineWaveData> data = new List<SineWaveData>();

            var wavedata = GetRandomSinewave(pad, amplitude, phase, pointCount, freq);
            data.Add(wavedata);

            return data;
        }

        private SineWaveData GetRandomSinewave(double pad, double amplitude, double phase, double pointCount, double freq)
        {
            SineWaveData res = new SineWaveData
            {
                Name = "123",
                X = new List<double>(),
                Y = new List<double>()
            };

            for (int i = 0, j = 0; i < pointCount; i++, j++)
            {
                amplitude = Math.Min(3, Math.Max(0.1, amplitude * (1 + (new Random().NextDouble() - 0.5) / 10)));
                freq = Math.Min(50, Math.Max(0.1, freq * (1 + (new Random().NextDouble() - 0.5) / 50)));

                double time = ((10 * i) / pointCount) + pad;
                double wn = (2 * Math.PI) / (pointCount / freq);

                double d = amplitude * Math.Sin(j * wn + phase);

                res.X.Add(time);
                res.Y.Add(d);
            }

            return res;
        }
    }
}
