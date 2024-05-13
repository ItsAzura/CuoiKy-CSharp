using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CK_CSharp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext dbContext;
        public EmployeeController(EmployeeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            if (!IsValidPhoneNumber(employee.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại không hợp lệ.");
                return View(employee); 
            }
            var newEmployee = new Employee
            {
                Name = employee.Name,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
            };

            await dbContext.Employees.AddAsync(newEmployee);

            await dbContext.SaveChangesAsync();

           return View();

            //return RedirectToAction("List", "Employee");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Biểu thức chính quy để kiểm tra số điện thoại
            string pattern = @"^(090|098|091|031|035|038)\d{7}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
