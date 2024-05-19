using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace CK_CSharp.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly EmployeeDbContext dbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AnnouncementController(EmployeeDbContext dbContext, IWebHostEnvironment hostingEnvironment)
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
        public async Task<IActionResult> Add(Announcement announcement, IFormFile Image)
        {
            if(string.IsNullOrEmpty(announcement.Title))
            {
                ModelState.AddModelError("Title", "Tiêu đề là cần thiết.");
                return View(announcement);
            }

            if(string.IsNullOrEmpty(announcement.Content))
            {
                ModelState.AddModelError("Content", "Nội dung là cần thiết.");
                return View(announcement);
            }

            if(!DateValidator(announcement.DatePosted))
            {
                ModelState.AddModelError("DatePosted", "Ngày tháng không hợp lệ.");
                return View(announcement);
            }

            var category = await dbContext.Categories.FindAsync(announcement.CategoryId);
            if(category == null)
            {
                ModelState.AddModelError("CategoryId", "Thể loại không hợp lệ.");
                return View(announcement);
            }
            announcement.CategoryName = category.Name;

            var employee = await dbContext.Employees.FindAsync(announcement.EmployeeId);
            if(employee == null)
            {
                ModelState.AddModelError("EmployeeId", "Nhân viên không hợp lệ.");
                return View(announcement);
            }
            announcement.EmployeeName = employee.Name;

            if (Image == null || Image.Length == 0)
            {
                ModelState.AddModelError("Image", "Ảnh là cần thiết.");
                return View(announcement);
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
            announcement.ImagePath = "/image/" + Image.FileName;

            var newAnnouncement = new Announcement
            {
                Title = announcement.Title,
                Content = announcement.Content,
                DatePosted = announcement.DatePosted,
                CategoryId = announcement.CategoryId,
                CategoryName = announcement.CategoryName,
                EmployeeId = announcement.EmployeeId,
                EmployeeName = announcement.EmployeeName,
                ImagePath = announcement.ImagePath,
                CreatedAt = DateTime.Now
            };

            try
            {
                await dbContext.announcements.AddAsync(newAnnouncement);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(announcement);
            }

            return RedirectToAction("List", "Announcement");
        }

        [HttpGet]
        public async Task<IActionResult> List(string searchString)
        {
            ViewData["AnnouncementCurrentFilter"] = searchString;

            var announcements = from a in dbContext.announcements
                            select a;

            if (!string.IsNullOrEmpty(searchString))
            {
                announcements = announcements.Where(a => a.Title.Contains(searchString));
            }

            return View(await announcements.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await dbContext.announcements.FindAsync(id);
            if(announcement == null)
            {
                return NotFound();
            }
            return View(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Announcement announcement, IFormFile Image) 
        {
            var AnnouncementUpdate = await dbContext.announcements.FindAsync(announcement.AnnouncementId);
            if (AnnouncementUpdate is not null)
            {
                if (string.IsNullOrEmpty(announcement.Title))
                {
                    ModelState.AddModelError("Title", "Tiêu đề là cần thiết.");
                    return View(announcement);
                }
                AnnouncementUpdate.Title = announcement.Title;

                if (string.IsNullOrEmpty(announcement.Content))
                {
                    ModelState.AddModelError("Content", "Nội dung là cần thiết.");
                    return View(announcement);
                }
                AnnouncementUpdate.Content = announcement.Content;

                if (!DateValidator(announcement.DatePosted))
                {
                    ModelState.AddModelError("DatePosted", "Ngày tháng không hợp lệ.");
                    return View(announcement);
                }
                AnnouncementUpdate.DatePosted = announcement.DatePosted;

                var category = await dbContext.Categories.FindAsync(announcement.CategoryId);
                if (category == null)
                {
                    ModelState.AddModelError("CategoryId", "Thể loại không hợp lệ.");
                    return View(announcement);
                }
                AnnouncementUpdate.CategoryName = category.Name;

                var employee = await dbContext.Employees.FindAsync(announcement.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError("EmployeeId", "Nhân viên không hợp lệ.");
                    return View(announcement);
                }
                AnnouncementUpdate.EmployeeName = employee.Name;

                if (Image == null || Image.Length == 0)
                {
                    ModelState.AddModelError("Image", "Ảnh là cần thiết.");
                    return View(announcement);
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
                AnnouncementUpdate.ImagePath = "/image/" + Image.FileName;
                
                AnnouncementUpdate.CategoryId = announcement.CategoryId;
                AnnouncementUpdate.EmployeeId = announcement.EmployeeId;

                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Announcement");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await dbContext.announcements.FindAsync(id);
            if(announcement == null)
            {
                return NotFound();
            }
            try
            {
                dbContext.announcements.Remove(announcement);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(announcement);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var announcement = await dbContext.announcements.FindAsync(id);
            if(announcement == null)
            {
                return NotFound();
            }
            return View(announcement);
        }

        private bool DateValidator(string date)
        {
            string Datepattern = @"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"; ;
            return Regex.IsMatch(date, Datepattern);
        }
    }
}
