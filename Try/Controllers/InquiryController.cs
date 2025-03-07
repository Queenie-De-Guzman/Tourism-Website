using Microsoft.AspNetCore.Mvc;

namespace Try.Controllers
{
    public class InquiryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
