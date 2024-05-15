using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CK_CSharp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly EmployeeDbContext dbContext;

        public CategoryController(EmployeeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            var newCategory = new Category
            {
                Name = category.Name
            };

            await dbContext.Categories.AddAsync(newCategory);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Category");
        }




    }
}
