using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CK_CSharp.Models
{
    public class Employee
    {
        [Key]
        [Display(Name = "Id Nhân Viên")]
        public int EmployeeId { get; set; }

        [Display(Name = "Tên Nhân Viên")]
        public string Name { get; set; }

        [Display(Name = "Địa Chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số Điện Thoại")]
        [RegularExpression(@"^(090|098|091|031|035|038)\d{7}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }


        [ForeignKey("Department")]
        [Display(Name = "Id Phòng Ban")]
        public int? DepartmentId { get; set; }

        public Department Department { get; set; }

        [Display(Name = "Tên Phòng Ban")]
        public string DepartmentName { get; set; }

        [ForeignKey("Company")]
        [Display(Name = "Id Công Ty")]
        public int? CompanyId { get; set; }

        public Company Company { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string ImagePath { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
