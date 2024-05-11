using System.ComponentModel.DataAnnotations;

namespace CK_CSharp.Models
{
    public class Company
    {
        [Key]
        [Display(Name = "Công Ty Id")]
        public int CompanyId { get; set; }

        [Display(Name = "Tên Công Ty")]
        public string Name { get; set; }

        [Display(Name = "Địa Chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số Điện Thoại")]
        [RegularExpression(@"^(090|098|091|031|035|038)\d{7}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public List<Department> Departments { get; set; }
    }
}
