using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CK_CSharp.Models
{
    public class Announcement
    {
        [Key]
        [Display(Name = "Id Bài đăng")]
        public int AnnouncementId { get; set; }

        [Display(Name = "Tiêu Đề")]
        [StringLength(150, MinimumLength = 20, ErrorMessage = "Tiêu đề phải có độ dài từ 20 đến 150 ký tự.")]
        public string Title { get; set; }

        [Display(Name = "Nội Dung")]
        public string Content { get; set; }

        [Display(Name = "Ngày Đăng")]
        [RegularExpression(@"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$", ErrorMessage = "Định dạng ngày không hợp lệ. Vui lòng nhập lại theo định dạng dd/MM/yyyy.")]
        public DateTime DatePosted { get; set; }

        [Display(Name = "Thể Loại")]
        public Category Category { get; set; }

        [ForeignKey("Category")]
        [Display(Name = "Id thể loại")]
        public int CategoryId { get; set; }

        public Employee Employee { get; set; }

        [ForeignKey("Employee")]
        [Display(Name = "Id nhân viên đăng bài")]
        public int EmployeeId { get; set; }

        public string Image { get; set; }
    }
}
