using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /*
     * GreetingController implements experimental MVC functionality for testing views and forms.
     * This controller provides a simple name input form and displays a greeting message.
     */
    public class GreetingController : Controller
    {
        /// <summary>
        /// Display the name input form
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Process the form submission and display greeting
        /// </summary>
        [HttpPost]
        public IActionResult Greet(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.ErrorMessage = "Please enter a name.";
                return View("Index");
            }

            ViewBag.Name = name;
            return View("Result");
        }
    }
}