using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var model = new Department();
            var companies = dbContext.Companies.ToList();
            //model.Companies = companies;
            return View(model);
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

            return View();
        }


    }
}
