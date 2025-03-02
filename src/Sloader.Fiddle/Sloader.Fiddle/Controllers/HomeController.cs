using Microsoft.AspNetCore.Mvc;
using Sloader.Engine;
using Sloader.Fiddle.Models;
using System.Diagnostics;

namespace Sloader.Fiddle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel();

            string sample = """
Crawler:
  FeedsToCrawl:
  - Url: http://blog.codeinside.eu/feed
    LoadSocialLinkCounters: true
    Key: Blog
""";

            viewModel.Input = sample;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Fiddle(HomeViewModel viewModel)
        {
            try
            {
                var runner = new SloaderRunner(Sloader.Config.SloaderConfig.Parse(viewModel.Input, new Dictionary<string, string>()));
                var crawlerRun = await runner.RunAllCrawlers();

                var json = crawlerRun.ToJson(Newtonsoft.Json.Formatting.Indented);
                viewModel.Output = json;
            }
            catch (Exception exc)
            {
                viewModel.HasError = true;
                viewModel.Output = exc.Message;
            }

            ModelState.Clear();

            return View("Index", viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
