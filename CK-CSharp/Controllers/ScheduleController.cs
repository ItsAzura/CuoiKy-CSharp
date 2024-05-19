using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace CK_CSharp.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly EmployeeDbContext dbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ScheduleController(EmployeeDbContext dbContext, IWebHostEnvironment hostingEnvironment) 
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
        public async Task<IActionResult> Add(Schedule schedule, IFormFile Image)
        {

            if (string.IsNullOrEmpty(schedule.Name))
            {
                ModelState.AddModelError("Name", "Tên Lịch trình là cần thiết.");
                return View(schedule);
            }

            if (!DateValidator(schedule.StartDate) )
            {
                ModelState.AddModelError("StartDate", "Ngày tháng không hợp lệ.");
                return View(schedule);
            }

            if (!DateValidator(schedule.EndDate))
            {
                ModelState.AddModelError("EndDate", "Ngày tháng không hợp lệ.");
                return View(schedule);
            }

            var employee = await dbContext.Employees.FindAsync(schedule.EmployeeId);
            if (employee == null)
            {
                ModelState.AddModelError("EmployeeId", "Nhân viên không hợp lệ.");
                return View(schedule);
            }
            schedule.EmployeeName = employee.Name;

            if (Image == null || Image.Length == 0)
            {
                ModelState.AddModelError("Image", "Ảnh là cần thiết.");
                return View(schedule);
            }
            //Tạo ra một đường dẫn tới thư mục "image" trong thư mục gốc
            var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "image");

            //kiểm tra xem một thư mục có tồn tại hay không dựa trên đường dẫn được cung cấp.
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            //tạo ra một đường dẫn tuyệt đối tới file ảnh.
            var filePath = Path.Combine(uploadPath, Image.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) //Tạo file ảnh (Nếu chưa tồn tại)
            {
                await Image.CopyToAsync(stream); //Lưu file ảnh vào thư mục "image"
            }

            //Lưu đường dẫn file ảnh vào database
            schedule.ImagePath = "/image/" + Image.FileName;

            var newSchedule = new Schedule
            {
                Name = schedule.Name,
                Description = schedule.Description,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                ImagePath = schedule.ImagePath,
                EmployeeId = schedule.EmployeeId,
                EmployeeName = schedule.EmployeeName,
                CreatedAt = DateTime.Now,
            };

            try
            {
                await dbContext.schedules.AddAsync(newSchedule);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(schedule);
            }

            return RedirectToAction("List", "Schedule");
        }

        [HttpGet]
        public async Task<IActionResult> List(string searchString)
        {
            ViewData["ScheduleCurrentFilter"] = searchString;

            var schedules = from s in dbContext.schedules
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                schedules = schedules.Where(s => s.Name.Contains(searchString));
            }

            return View(await schedules.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await dbContext.schedules.FindAsync(id);
            return View(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Schedule schedule, IFormFile Image)
        {
            var scheduleToUpdate = await dbContext.schedules.FindAsync(schedule.ScheduleId);

            if (scheduleToUpdate is not null)
            {
                if (string.IsNullOrEmpty(schedule.Name))
                {
                    ModelState.AddModelError("Name", "Tên Lịch trình là cần thiết.");
                    return View(schedule);
                }

                if (!DateValidator(schedule.StartDate))
                {
                    ModelState.AddModelError("StartDate", "Ngày tháng không hợp lệ.");
                    return View(schedule);
                }

                if (!DateValidator(schedule.EndDate))
                {
                    ModelState.AddModelError("EndDate", "Ngày tháng không hợp lệ.");
                    return View(schedule);
                }

                var employee = await dbContext.Employees.FindAsync(schedule.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError("EmployeeId", "Nhân viên không hợp lệ.");
                    return View(schedule);
                }
                schedule.EmployeeName = employee.Name;
                scheduleToUpdate.EmployeeName = schedule.EmployeeName;

                if (Image != null && Image.Length > 0)
                {
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

                    scheduleToUpdate.ImagePath = "/image/" + Image.FileName;
                }

                scheduleToUpdate.Name = schedule.Name;
                scheduleToUpdate.Description = schedule.Description;
                scheduleToUpdate.StartDate = schedule.StartDate;
                scheduleToUpdate.EndDate = schedule.EndDate;
                scheduleToUpdate.EmployeeId = schedule.EmployeeId;
                

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Schedule");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await dbContext.schedules.FirstOrDefaultAsync(x => x.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            try
            {
                dbContext.schedules.Remove(schedule);
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
            var schedule = await dbContext.schedules.FindAsync(id);
            return View(schedule);
        }

        private bool DateValidator(string date)
        {
            string Datepattern = @"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"; ;
            return Regex.IsMatch(date, Datepattern);
        }

    }
}
