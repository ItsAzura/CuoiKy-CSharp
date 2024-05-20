using Azure.Core;
using CK_CSharp.Data;
using CK_CSharp.Models;
using CK_CSharp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//T?o m?t ??i t??ng JwtConfiguration m?i.
var jwtConfig = new JwtConfiguration();
// ??c các c?u hình t? appsettings.json và gán vào jwtConfig.
builder.Configuration.GetSection("Jwt").Bind(jwtConfig);
// ??ng ký jwtConfig vào d?ch v?.
builder.Services.AddSingleton(jwtConfig);
// ??ng ký AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EmployeeDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Thêm d?ch v? Identity vào d?ch v?.
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;

    // Password
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<EmployeeDbContext>();

// ??c c?u hình Jwt t? appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt"); 

// ??ng ký d?ch v? xác th?c
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Xác th?c m?c ??nh 
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Thách th?c m?c ??nh
})
 .AddJwtBearer(options => // Thêm xác th?c JWT
 {
      options.TokenValidationParameters = new TokenValidationParameters() // C?u hình các tham s? ki?m tra token
      {
           ValidateActor = true,
           ValidateIssuer = true,
           ValidateAudience = true,
           RequireExpirationTime = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = jwtSettings["Issuer"],
           ValidAudience = jwtSettings["Audience"],
           ClockSkew = TimeSpan.Zero,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
      };

       options.Events = new JwtBearerEvents() // C?u hình s? ki?n xác th?c
       {
            OnMessageReceived = context =>
            {
                 context.Token = context.Request.Cookies["AccessToken"];
                 return Task.CompletedTask;
            }
       };
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // S? d?ng xác th?c 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.SeedDataAsync(); // Thêm d? li?u vào c? s? d? li?u

app.Run();
