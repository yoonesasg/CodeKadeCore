using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Controllers
{
    public class NotFound : Controller
    {
        [Route("NotFound")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
