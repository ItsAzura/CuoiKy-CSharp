using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CK_CSharp.Controllers
{
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
        public async Task<IActionResult> List()
        {
            var departments = await dbContext.Departments.ToListAsync();
            return View(departments);
        }


    }
}
