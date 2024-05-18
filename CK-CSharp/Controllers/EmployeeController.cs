using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace CK_CSharp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext dbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmployeeController(EmployeeDbContext dbContext, IWebHostEnvironment hostingEnvironment)
        {
            this.dbContext = dbContext;
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee, IFormFile Image)
        {
            // Kiểm tra số điện thoại hợp lệ
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

            if (Image == null || Image.Length == 0)
            {
                ModelState.AddModelError("Image", "Ảnh là cần thiết.");
                return View(employee);
            }
            var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "image");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var filePath = Path.Combine(uploadPath, Image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Image.CopyToAsync(stream);
            }
            employee.ImagePath = "/image/" + Image.FileName;

            var newEmployee = new Employee
            {
                Name = employee.Name,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.DepartmentName,
                ImagePath = employee.ImagePath,
                CreatedAt = DateTime.Now
            };

            try
            {
                await dbContext.Employees.AddAsync(newEmployee);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employee);
            }

            return RedirectToAction("List", "Employee");
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
                    .FirstOrDefault() ?? "Unknown",
                ImagePath = employee.ImagePath,
                CreatedAt = employee.CreatedAt
            }).ToList(); 

            return View(employeeViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await dbContext.Employees.FindAsync(id);
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee, IFormFile Image)
        {
            var employeeToUpdate = await dbContext.Employees.FindAsync(employee.EmployeeId);

            // Nếu nhân viên tồn tại
            if (employeeToUpdate is not null) 
            {
                if (!IsValidPhoneNumber(employee.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại không hợp lệ.");
                    return View(employee);
                }
                employeeToUpdate.PhoneNumber = employee.PhoneNumber;

                if (Image == null || Image.Length == 0)
                {
                    ModelState.AddModelError("Image", "Ảnh là cần thiết.");
                    return View(employee);
                }
                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "image");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var filePath = Path.Combine(uploadPath, Image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                employeeToUpdate.ImagePath = "/image/" + Image.FileName;

                employeeToUpdate.Name = employee.Name;
                employeeToUpdate.Address = employee.Address;
                employeeToUpdate.Email = employee.Email;
                employeeToUpdate.DepartmentId = employee.DepartmentId;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Employee");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            try
            {
                dbContext.Employees.Remove(employee);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet]   
        public async Task<IActionResult> Detail(int id)
        {
            var employee = await dbContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Biểu thức chính quy để kiểm tra số điện thoại
            string pattern = @"^(090|098|091|031|035|038)\d{7}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
