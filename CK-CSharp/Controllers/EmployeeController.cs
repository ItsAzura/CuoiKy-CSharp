using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace CK_CSharp.Controllers
{
    [Authorize(Roles = "Admin")]
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

            if(!DateValidator(employee.Dob))
            {
                ModelState.AddModelError("Dob", "Ngày tháng không hợp lệ.");
                return View(employee);
            }

            if(!DateValidator(employee.StartTime))
            {
                ModelState.AddModelError("StartTime", "Ngày tháng không hợp lệ.");
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
                Dob = employee.Dob,
                PhoneNumber = employee.PhoneNumber,
                Salary = employee.Salary,
                Email = employee.Email,
                StartTime = employee.StartTime,
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
        public async Task<IActionResult> List(string searchString, long? minSalary, long? maxSalary, string sortOrder)
        {
            //Lấy giá trị từ view để giữ giá trị filter khi chuyển trang
            ViewData["EmployeeCurrentFilter"] = searchString;
            ViewData["MinSalary"] = minSalary;
            ViewData["MaxSalary"] = maxSalary;
            ViewData["CurrentSort"] = sortOrder;

            //Tạo truy vấn LINQ để lấy tất cả thông tin từ bảng employees.
            var employees = from e in dbContext.Employees
                            select e;

            //Kiểm tra xem searchString có giá trị không
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Name.Contains(searchString));
            }

            //Kiểm tra xem minSalary có giá trị không
            if (minSalary.HasValue)
            {
                employees = employees.Where(e => e.Salary >= minSalary.Value);
            }

            //Kiểm tra xem maxSalary có giá trị không
            if (maxSalary.HasValue)
            {
                employees = employees.Where(e => e.Salary <= maxSalary.Value);
            }

            //Sắp xếp theo tên
            employees = sortOrder switch
            {
                "name_desc" => employees.OrderByDescending(e => e.Name),
                _ => employees.OrderBy(e => e.Name),
            };

            //Thực thi truy vấn LINQ
            var employeeList = await employees.ToListAsync(); 

            //Tạo một danh sách mới để chứa thông tin nhân viên
            var employeeViewModels = employeeList.Select(employee => new Employee
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

            //Trả về view với danh sách nhân viên
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

                if (!DateValidator(employee.Dob))
                {
                    ModelState.AddModelError("Dob", "Ngày tháng không hợp lệ.");
                    return View(employee);
                }
                employeeToUpdate.Dob = employee.Dob;

                if (!DateValidator(employee.StartTime))
                {
                    ModelState.AddModelError("StartTime", "Ngày tháng không hợp lệ.");
                    return View(employee);
                }
                employeeToUpdate.StartTime = employee.StartTime;

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
                employeeToUpdate.Salary = employee.Salary;
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

        private bool DateValidator(string date)
        {
            string Datepattern = @"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"; ;
            return Regex.IsMatch(date, Datepattern);
        }
    }
}
