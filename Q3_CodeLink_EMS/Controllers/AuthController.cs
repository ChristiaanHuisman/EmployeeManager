using Microsoft.AspNetCore.Mvc;
using Q3_CodeLink_EMS.Dtos.AdminUser;
using Q3_CodeLink_EMS.Services;

namespace Q3_CodeLink_EMS.Controllers
{
    public class AuthController : Controller
    {
        private readonly AdminUserService adminUserService;

        public AuthController(AdminUserService adminUserService)
        {
            this.adminUserService = adminUserService;
        }

        // GET: Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost, ValidateAntiForgeryToken] // Preventing Cross-Site Request Forgery
        public async Task<IActionResult> Register(RegisterAdminUserDto dto)
        {
            // Checking if the submitted model passes all its validation rules
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool success = await adminUserService.RegisterAdminUser(dto);

            if (!success)
            {
                // Info popup message options
                TempData["Message"] = "An account with this email already exists.";
                TempData["MessageType"] = "danger";
                return View(dto);
            }

            // Info popup message options
            TempData["Message"] = "Registration successful. You can now login.";
            TempData["MessageType"] = "success";
            return RedirectToAction(nameof(Login));
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost, ValidateAntiForgeryToken] // Preventing Cross-Site Request Forgery
        public async Task<IActionResult> Login(LoginAdminUserDto dto)
        {
            // Checking if the submitted model passes all its validation rules
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
                
            bool success = await adminUserService.LoginAdminUser(dto);

            if (!success)
            {
                ViewData["LoginError"] = "Invalid email or password.";
                return View(dto);
            }

            // Store logged in user Id and Role in this session
            var adminUser = await adminUserService.GetByEmail(dto.EmailAddress);
            HttpContext.Session.SetString("AdminUserId", adminUser.Id.ToString());
            HttpContext.Session.SetString("AdminUserRole", adminUser.Role.ToString());

            // Info popup message options
            TempData["Message"] = "Login successful.";
            TempData["MessageType"] = "success";

            // Redirect to employee index after login
            return RedirectToAction("Index", "Employee");
        }

        // POST: Auth/Logout
        [HttpPost, ValidateAntiForgeryToken] // Preventing Cross-Site Request Forgery
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUserId");
            // Info popup message options
            TempData["Message"] = "You have been signed out.";
            TempData["MessageType"] = "info";
            return RedirectToAction(nameof(Login));
        }
    }
}
