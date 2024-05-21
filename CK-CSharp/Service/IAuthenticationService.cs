using CK_CSharp.Models;
using Microsoft.AspNetCore.Identity;

namespace CK_CSharp.Service
{
    public interface IAuthenticationService
    {
        Task<bool> AddUserRole(IdentityUser user, string role); // Thêm role đó có thành công không?
        Task<(bool IsSuccess, IdentityUser? User)> Login(UserLogin credentials); // Đăng nhập có thành công không? Trả về user nếu thành công
        Task Logout(); // Đăng xuất
        Task<(bool IsSuccess, IdentityUser? User, IEnumerable<string>? errors)> RegisterUser(UserRegister user); // Đăng ký user có thành công không? Trả về user nếu thành công
        Task<string> GenerateToken(IdentityUser user, JwtConfiguration jwtConfig); // Tạo token cho user dựa trên config
        void WriteAccessToken(string accessToken); // Ghi token vào file
    }
}
