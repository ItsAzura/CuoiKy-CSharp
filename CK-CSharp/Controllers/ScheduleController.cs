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
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(EmployeeDbContext dbContext, IWebHostEnvironment hostingEnvironment, ILogger<ScheduleController> logger) 
        {
            this.dbContext = dbContext;
            this._hostingEnvironment = hostingEnvironment;
            _logger = logger;
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
        public async Task<IActionResult> List(string searchString,string startDate, string endDate)
        {
            ViewData["ScheduleCurrentFilter"] = searchString;
            ViewData["StartDateFilter"] = startDate;
            ViewData["EndDateFilter"] = endDate;

            var schedules = from s in dbContext.schedules
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                schedules = schedules.Where(s => s.Name.Contains(searchString));
            }

            var schedulesList = await schedules.AsNoTracking().ToListAsync();

            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedStartDate))
                {
                    schedulesList = schedulesList.Where(s => DateTime.ParseExact(s.StartDate, "dd/MM/yyyy", null) >= parsedStartDate).ToList();
                    _logger.LogInformation("Filtered by start date ({StartDate}), remaining count: {Count}", startDate, schedulesList.Count);
                }
                else
                {
                    ModelState.AddModelError("startDate", "Định dạng ngày không hợp lệ. Vui lòng nhập lại theo định dạng dd/MM/yyyy.");
                }
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEndDate))
                {
                    schedulesList = schedulesList.Where(s => DateTime.ParseExact(s.EndDate, "dd/MM/yyyy", null) <= parsedEndDate).ToList();
                    _logger.LogInformation("Filtered by end date ({EndDate}), remaining count: {Count}", endDate, schedulesList.Count);
                }
                else
                {
                    ModelState.AddModelError("endDate", "Định dạng ngày không hợp lệ. Vui lòng nhập lại theo định dạng dd/MM/yyyy.");
                }
            }

            return View(schedulesList);
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
