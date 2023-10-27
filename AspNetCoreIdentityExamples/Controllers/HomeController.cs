using AspNetCoreIdentityExamples.Models;
using AspNetCoreIdentityExamples.ViewModels;
using EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreIdentityExamples.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
           
            return View();
           
        }


        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(AttachmentViewModel model)
        {

            //var files = new FormFileCollection();

            //foreach (var file in model.Attachments)
            //{
            //    var formFile = new FormFile(
            //        baseStream: file.OpenReadStream(),
            //        baseStreamOffset: 0,
            //        length: file.Length,
            //        name: file.Name,
            //        fileName: file.FileName
            //       );

            //    files.Add(formFile);
            //}
            var message = new Message(
                    new string[] { "yimos464@hondabbs.com" },
                    "Test mail with Attachments", "" +
                    "This is the content from our mail with attachments.",
                    model.Attachments);
            await _emailSender.SendEmailAsync(message);
            return Content("Has Sent");
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
