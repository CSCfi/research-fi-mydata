using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /*
     * AiPocController implements experimental MVC functionality for testin AI feature
     */
    public class AiPocController : Controller
    {
        /// <summary>
        /// Display the name input form
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Process the form submission and display result
        /// </summary>
        [HttpPost]
        public IActionResult Greet(string orcid)
        {
            if (string.IsNullOrWhiteSpace(orcid))
            {
                ViewBag.ErrorMessage = "Please enter ORCID.";
                return View("Index");
            }

            ViewBag.Orcid = orcid;
            return View("Result");
        }
    }
}