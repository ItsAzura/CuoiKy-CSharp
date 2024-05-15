using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CK_CSharp.Controllers
{
    public class CompanyController : Controller
    {
        private readonly EmployeeDbContext dbContext;

        public CompanyController(EmployeeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Company companys)
        {
            // Kiểm tra xem số điện thoại có đúng định dạng không
            if (!IsValidPhoneNumber(companys.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại không hợp lệ.");
                return View(companys); // Trả về view với dữ liệu đã nhập và thông báo lỗi
            }
            var company = new Company
            {
                Name = companys.Name,
                Address = companys.Address,
                PhoneNumber = companys.PhoneNumber,
                Email = companys.Email,
            };

            await dbContext.Companies.AddAsync(company);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Company");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var companies = await dbContext.Companies.ToListAsync();
            return View(companies);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var company = await dbContext.Companies.FindAsync(id);
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Company companys)
        {
            var company = await dbContext.Companies.FindAsync(companys.CompanyId);

            // Nếu Company có tồn tại
            if (company is not null) 
            {
                if (!IsValidPhoneNumber(companys.PhoneNumber)) // Kiểm tra số điện thoại có đúng định dạng không?
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại không hợp lệ.");
                    return View(companys);
                }

                company.Name = companys.Name;
                company.Address = companys.Address;
                company.PhoneNumber = companys.PhoneNumber;
                company.Email = companys.Email;

                await dbContext.SaveChangesAsync();

            }

            return RedirectToAction("List", "Company");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await dbContext.Companies.FirstOrDefaultAsync(x => x.CompanyId == id);
            if (company == null)
            {
                return NotFound(); // Trả về HTTP 404 Not Found nếu không tìm thấy công ty
            }

            try
            {
                dbContext.Companies.Remove(company);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ở đây, ví dụ: ghi log, hiển thị thông báo lỗi cho người dùng
                return BadRequest(ex.Message); // Trả về HTTP 400 Bad Request nếu có lỗi xảy ra
            }

            return Ok(); // Trả về HTTP 200 OK để xác nhận xóa thành công
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var company = await dbContext.Companies.FindAsync(id);
            return View(company);
        }


        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Biểu thức chính quy để kiểm tra số điện thoại
            string pattern = @"^(090|098|091|031|035|038)\d{7}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
