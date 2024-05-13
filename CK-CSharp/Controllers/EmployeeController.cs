using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            // Truy vấn Department từ DepartmentId
            var department = await dbContext.Departments.FindAsync(employee.DepartmentId);
            if (department == null)
            {
                ModelState.AddModelError("DepartmentId", "Phòng ban không hợp lệ.");
                return View(employee);
            }
            employee.DepartmentName = department.Name;

            var newEmployee = new Employee
            {
                Name = employee.Name,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.DepartmentName
            };

            await dbContext.Employees.AddAsync(newEmployee);

            await dbContext.SaveChangesAsync();

           return View();

            //return RedirectToAction("List", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var employees = await dbContext.Employees.ToListAsync();
            var employeeViewModels = employees.Select(employee => new Employee
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                DepartmentName = dbContext.Departments
                    .Where(d => d.DepartmentId == employee.DepartmentId)
                    .Select(d => d.Name)
                    .FirstOrDefault() ?? "Unknown"
            }).ToList();

            return View(employeeViewModels);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Biểu thức chính quy để kiểm tra số điện thoại
            string pattern = @"^(090|098|091|031|035|038)\d{7}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
