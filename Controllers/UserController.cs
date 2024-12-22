using Microsoft.AspNetCore.Mvc;

namespace WebProje.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserDashboard()
        {
            // Session'dan rolü al
            var role = HttpContext.Session.GetString("Role");

            // Eğer kullanıcı adminse, erişim reddedilir
            if (string.IsNullOrEmpty(role) || role == "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


    }
}
