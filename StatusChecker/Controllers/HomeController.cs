using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StatusChecker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StatusChecker.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult CheckStatus(LinksViewModel model) {
            if (ModelState.IsValid && model.InputLinks != null && model.Links.Any()) {
                return View("StatusView", model);
            }
            return Error();
        }

        public async Task<IActionResult> GetSiteStatus(string url) {
            try {
                if (ModelState.IsValid && !string.IsNullOrWhiteSpace(url)) {
                    using (var client = new HttpClient()) {
                        var response = await client.GetAsync(url);
                        var result = new List<StatusResponseViewModel>();
                        if (response.IsSuccessStatusCode) {
                            var html = await response.Content.ReadAsStringAsync();
                            var title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase)?.Groups["Title"]?.Value ?? "Title not found";
                            var vm = new StatusResponseViewModel {
                                Status = (int)response.StatusCode,
                                Title = title
                            };
                            result.Add(vm);
                            return PartialView("StatusResponseView", vm);
                        }
                    }
                }
                return PartialView("StatusResponseView", new StatusResponseViewModel {
                    Title = $"Error with {url}",
                    Status = 400
                });
            } catch (Exception ex) {
                _logger?.LogError($"{DateTime.UtcNow} | ERROR | {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return Error();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
