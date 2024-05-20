using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CK_CSharp.Service
{
    public class AuthenticationHelper
    {
        public static async Task<List<Claim>> GetClaims(IdentityUser user, UserManager<IdentityUser> userManager)
        {
            // Tạo một danh sách mới chứa một claim với loại là ClaimTypes.Name và giá trị là tên người dùng.
            var claims = new List<Claim> 
        {
            new Claim(ClaimTypes.Name, user.UserName)
        };

            //Sử dụng UserManager để lấy danh sách các vai trò của người dùng.
            var roles = await userManager.GetRolesAsync(user);

            //Duyệt qua danh sách các vai trò và thêm mỗi vai trò như một claim với loại là ClaimTypes.Role vào danh sách claims.
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Trả về danh sách claims.
            return claims; 
        }
    }
}
