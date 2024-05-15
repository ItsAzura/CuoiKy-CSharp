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
