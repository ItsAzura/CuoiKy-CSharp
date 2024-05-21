using CK_CSharp.Data;
using CK_CSharp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;

namespace CK_CSharp.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly EmployeeDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContext;

        public AuthenticationService(
            EmployeeDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IHttpContextAccessor httpContext
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _httpContext = httpContext;
        }

        //Phương thức này thêm một vai trò cho người dùng và trả về kết quả thành công hay thất bại.
        public async Task<bool> AddUserRole(IdentityUser user, string role)
        {
            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);

            return addToRoleResult.Succeeded;
        }

        //Phương thức này kiểm tra thông tin đăng nhập và trả về kết quả thành công hay thất bại cùng với thông tin người dùng.
        public async Task<(bool IsSuccess, IdentityUser? User)> Login(UserLogin credentials)
        {
            // Tìm người dùng theo email hoặc tên người dùng.
            var user = await _userManager.FindByEmailAsync(credentials.UserOrEmail) ?? 
                       await _userManager.FindByNameAsync(credentials.UserOrEmail);

            // Nếu người dùng tồn tại
            if (user is not null) 
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, credentials.Password, false); // Kiểm tra mật khẩu
                // Nếu đăng nhập thành công
                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(user, credentials.Password, false, false); // Đăng nhập người dùng vào hệ thống
                    return (true, user); // Trả về kết quả thành công và thông tin người dùng
                }
            }

            return (false, null); // Trả về kết quả thất bại
        }

        //Phương thức này đăng xuất người dùng và xóa cookie "AccessToken"
        public async Task Logout() 
        {
            await _signInManager.SignOutAsync(); // Đăng xuất người dùng
            _httpContext.HttpContext?.Response.Cookies.Delete("AccessToken"); // Xóa "AccessToken"
        }

        //Đăng Ký người dùng
        public async Task<(bool IsSuccess, IdentityUser? User, IEnumerable<string>? errors)> RegisterUser(UserRegister user) 
        {
            // Tạo một đối tượng IdentityUser mới
            var identityUser = new IdentityUser() 
            {
                Email = user.UserEmail,
                UserName = user.UserName
            };

            // Tạo người dùng mới với mật khẩu được mã hóa bằng SHA256
            var result = await _userManager.CreateAsync(identityUser, user.Password);
            // Lấy danh sách lỗi nếu có
            var errors = result.Errors.Select(e => e.Description);

            // Nếu tạo người dùng thành công
            if (result.Succeeded) 
            {
                var addRoleResult = await AddUserRole(identityUser, "Admin"); // Thêm vai trò Admin cho người dùng
                if (addRoleResult)
                    return (true, identityUser, errors); // Trả về kết quả thành công và thông tin người dùng

                return (false, null, ["Failed to add role to user."]); // Trả về kết quả thất bại
            }

            return (false, null, errors); // Trả về kết quả thất bại
        }

        //Phương thức này tạo một token JWT cho người dùng
        public async Task<string> GenerateToken(IdentityUser user, JwtConfiguration jwtConfig) 
        {
            var claims = await AuthenticationHelper.GetClaims(user, _userManager); // Lấy danh sách claims của người dùng 

            var vSymmetricSecurityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key)); // Tạo một security key từ key trong config 

            var signingCred = new Microsoft.IdentityModel.Tokens.SigningCredentials(vSymmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature); // Tạo signing credentials từ security key
             
            // Tạo một SecurityTokenDescriptor
            var securityTokenDescriptor = new SecurityTokenDescriptor() //
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtConfig.Issuer,
                Audience = jwtConfig.Audience,
                Expires = DateTime.Now.AddDays(jwtConfig.ExpireYears),
                SigningCredentials = signingCred
            };

            // Tạo một JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler(); 
            // Tạo một token từ SecurityTokenDescriptor
            var token = tokenHandler.CreateToken(securityTokenDescriptor); 
            // Trả về token dưới dạng string
            return tokenHandler.WriteToken(token);
        }

        //Phương thức này ghi token JWT vào cookie "AccessToken" của phản hồi HTTP.
        public void WriteAccessToken(string accessToken)
        {
            // Ghi token vào cookie "AccessToken" của phản hồi HTTP
            _httpContext.HttpContext?.Response.Cookies.Append("AccessToken", accessToken, new CookieOptions 
            {
                Expires = DateTime.Now.AddDays(60), 
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
            });
        }
    }
}

