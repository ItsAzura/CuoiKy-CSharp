using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CK_CSharp.Models
{
    public class Schedule
    {
        [Key]
        [Display(Name = "Id Lịch Trình")]
        public int ScheduleId { get; set; }

        [Display(Name = "Tên Lịch Trình")]
        [StringLength(150, MinimumLength = 20, ErrorMessage = "Tiêu đề phải có độ dài từ 20 đến 150 ký tự.")]
        public string Name { get; set; }

        [Display(Name = "Mô Tả")]
        public string Description { get; set; }

        [Display(Name = "Thời gian bắt đầu")]
        [RegularExpression(@"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$", ErrorMessage = "Định dạng ngày không hợp lệ. Vui lòng nhập lại theo định dạng dd/MM/yyyy.")]
        public string StartDate { get; set; }

        [Display(Name = "Thời gian kết thúc")]
        [RegularExpression(@"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$", ErrorMessage = "Định dạng ngày không hợp lệ. Vui lòng nhập lại theo định dạng dd/MM/yyyy.")]
        public string EndDate { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        [Display(Name = "Đường dẫn ảnh")]
        public string ImagePath { get; set; }

        public Employee Employee { get; set; }

        [ForeignKey("Employee")]
        [Display(Name = "Id nhân viên")]
        public int EmployeeId { get; set; }

        [Display(Name = "Tên nhân viên")]
        public string? EmployeeName { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
