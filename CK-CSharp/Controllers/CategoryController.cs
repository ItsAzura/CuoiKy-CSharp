using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                Name = category.Name,
                Description = category.Description
            };

            await dbContext.Categories.AddAsync(newCategory);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Category");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var categories = await dbContext.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await dbContext.Categories.FindAsync(id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            var categoryToUpdate = await dbContext.Categories.FindAsync(category.CategoryId);
            if (categoryToUpdate is not null)
            {
                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Description = category.Description;

                await dbContext.SaveChangesAsync();
            }
           

            return RedirectToAction("List", "Category");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            try
            {
                dbContext.Categories.Remove(category);
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
            var category = await dbContext.Categories.FindAsync(id);
            return View(category);
        }

    }
}
