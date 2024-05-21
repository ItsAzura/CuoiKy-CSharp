using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CK_CSharp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly EmployeeDbContext dbContext;

        public DepartmentController(EmployeeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Department departments)
        {
                var department = new Department
                {
                    Name = departments.Name,
                    Description = departments.Description,
                    CompanyId = departments.CompanyId
                };

                await dbContext.Departments.AddAsync(department);

                await dbContext.SaveChangesAsync();

                return RedirectToAction("List", "Department");
        }

        [HttpGet]
        public async Task<IActionResult> List(string searchString, string sortOrder)
        {
            ViewData["DepartmentCurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            var departments = from d in dbContext.Departments select d;
            
            if (!string.IsNullOrEmpty(searchString))
            {
                departments = departments.Where(s => s.Name.Contains(searchString));
            
            }

            // Sắp xếp theo tên
            departments = sortOrder switch
            {
                "name_desc" => departments.OrderByDescending(c => c.Name),
                _ => departments.OrderBy(c => c.Name),
            };

            return View(await departments.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Department departments)
        {
            var department = await dbContext.Departments.FindAsync(departments.DepartmentId);
            if(department is not null)
            {
                department.Name = departments.Name;
                department.Description = departments.Description;
                department.CompanyId = departments.CompanyId;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Department");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            try
            {
                dbContext.Departments.Remove(department);
                await dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            return View(department);
        }


    }
}
