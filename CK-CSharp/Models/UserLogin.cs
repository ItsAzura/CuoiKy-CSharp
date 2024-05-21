using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CK_CSharp.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Email hoặc UserName là bắt buộc")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Tên người dùng phải có ít nhất 6 ký tự")]
        [DisplayName("Email hoặc UserName")]
        public string UserOrEmail { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        public string? ErrorMessage { get; set; }
    }
}