using Microsoft.AspNetCore.Mvc;

namespace SE_Project___POS_System.Extensions
{
    public static class ControllerRoleGuards
    {
        public static IActionResult? RequireManagerFromSession(this Controller controller)
        {
            var employeeId = controller.HttpContext.Session.GetString("EmployeeId");
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return controller.RedirectToAction("Login", "Auth");
            }

            var employeeRole = controller.HttpContext.Session.GetString("EmployeeRole");
            if (!string.Equals(employeeRole, "Manager", StringComparison.OrdinalIgnoreCase))
            {
                return controller.Forbid();
            }

            return null;
        }
    }
}
