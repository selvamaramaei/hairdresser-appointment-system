using Microsoft.AspNetCore.Mvc;

namespace WebProje.Controllers
{
    public class AdminController : Controller
    {
        // Admin paneline yönlendirme
        public IActionResult AdminDashboard()
        {
            // Session'dan rolü al
            var role = HttpContext.Session.GetString("Role");

            // Eğer kullanıcı admin değilse, erişim reddedilir
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
