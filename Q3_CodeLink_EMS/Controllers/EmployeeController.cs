using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Q3_CodeLink_EMS.Models;
using Q3_CodeLink_EMS.Services;

namespace Q3_CodeLink_EMS.Controllers
{
    public class EmployeeController : Controller
    {
        // Helper method to check if the user is logged in
        private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUserId"));

        // Override OnActionExecuting to redirect if a user is not logged in
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Checking if user is logged in
            if (!IsLoggedIn())
            {
                context.Result = RedirectToAction("Login", "Auth");
            }
            base.OnActionExecuting(context);
        }

        private readonly EmployeeService employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // GET: Employee/Index
        [HttpGet]
        public async Task<IActionResult> Index(string? searchField, string? searchValue)
        {
            List<Employee> employees;

            // Checking if something is being searched
            if (!string.IsNullOrEmpty(searchField))
            {
                string field = searchField.Trim().ToLower();
                // Switch case for differnt search scenarios
                employees = field switch
                {
                    "id" => await employeeService.SearchById(Guid.TryParse(searchValue, out var guid) ? guid : Guid.Empty), // Checking if entered Guid can be converted to an actual Guid
                    "fullname" => await employeeService.SearchByFullName(searchValue),
                    "emailaddress" => await employeeService.SearchByEmailAddress(searchValue),
                    "jobtitle" => await employeeService.SearchByJobTitle(searchValue),
                    "department" => await employeeService.SearchByDepartment(searchValue),
                    _ => await employeeService.GetAllEmployees()
                };
            }
            else
            {
                employees = await employeeService.GetAllEmployees();
            }

            // Keeping selected values for re-populating the form
            ViewBag.SearchField = searchField;
            ViewBag.SearchValue = searchValue;

            return View(employees);
        }

        // GET: Employee/Dummy
        [HttpGet]
        public async Task<IActionResult> Dummy()
        {
            bool success = await employeeService.PreloadDummyData();
            // Info popup message options
            TempData["Message"] = success ? "Dummy employee records added successfully." : "Failed to add dummy employee records.";
            TempData["MessageType"] = success ? "success" : "danger";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employee/Add
        [HttpGet]
        public IActionResult Add()
        {
            // Checking user role
            var role = HttpContext.Session.GetString("AdminUserRole");
            if (role != "SuperAdmin")
            {
                // Info popup message options
                TempData["Message"] = "Only specified admins can access the reqested page.";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // POST: Employee/Add
        [HttpPost, ValidateAntiForgeryToken] // Preventing Cross-Site Request Forgery
        public async Task<IActionResult> Add(Employee employee)
        {
            // Checking user role
            var role = HttpContext.Session.GetString("AdminUserRole");
            if (role != "SuperAdmin")
            {
                // Info popup message options
                TempData["Message"] = "Only specified admins can perform the reqested action.";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(Index));
            }

            // Checking if the submitted model passes all its validation rules
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            bool success = await employeeService.CreateEmployee(employee);
            // Info popup message options
            TempData["Message"] = success ? "Employee record successfully added." : "Failed to add employee record. Email may already be used.";
            TempData["MessageType"] = success ? "success" : "danger";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employee/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Checking user role
            var role = HttpContext.Session.GetString("AdminUserRole");
            if (role != "SuperAdmin")
            {
                // Info popup message options
                TempData["Message"] = "Only specified admins can access the reqested page.";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(Index));
            }

            var employees = await employeeService.SearchById(id);

            // Checking if only one employee record is found
            if (employees.Count == 0)
            {
                // Info popup message options
                TempData["Message"] = "Employee record not found.";
                TempData["MessageType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
            if (employees.Count > 1)
            {
                // Info popup message options
                TempData["Message"] = "Multiple employee records found with the same ID.";
                TempData["MessageType"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            return View(employees.First());
        }

        // POST: Employee/Edit
        [HttpPost, ValidateAntiForgeryToken] // Preventing Cross-Site Request Forgery
        public async Task<IActionResult> Edit(Employee employee)
        {
            // Checking user role
            var role = HttpContext.Session.GetString("AdminUserRole");
            if (role != "SuperAdmin")
            {
                // Info popup message options
                TempData["Message"] = "Only specified admins can perform the reqested action.";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(Index));
            }

            // Checking if the submitted model passes all its validation rules
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            bool success = await employeeService.UpdateEmployee(employee);
            // Info popup message options
            TempData["Message"] = success ? "Employee record successfully updated." : "Failed to update employee record.";
            TempData["MessageType"] = success ? "success" : "danger";
            return RedirectToAction(nameof(Index));
        }

        // POST: Employee/Delete
        [HttpPost, ValidateAntiForgeryToken] // Preventing Cross-Site Request Forgery
        public async Task<IActionResult> Delete(Guid id)
        {
            // Checking user role
            var role = HttpContext.Session.GetString("AdminUserRole");
            if (role != "SuperAdmin")
            {
                // Info popup message options
                TempData["Message"] = "Only specified admins can perform the reqested action.";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(Index));
            }

            bool success = await employeeService.DeleteEmployee(id);
            // Info popup message options
            TempData["Message"] = success ? "Employee record successfully deleted." : "Failed to delete employee record.";
            TempData["MessageType"] = success ? "success" : "danger";
            return RedirectToAction(nameof(Index));
        }
    }
}
