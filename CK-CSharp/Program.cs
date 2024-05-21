using Azure.Core;
using CK_CSharp.Data;
using CK_CSharp.Models;
using CK_CSharp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//T?o m?t ??i t??ng JwtConfiguration m?i.
var jwtConfig = new JwtConfiguration();
// ??c c�c c?u h�nh t? appsettings.json v� g�n v�o jwtConfig.
builder.Configuration.GetSection("Jwt").Bind(jwtConfig);
// ??ng k� jwtConfig v�o d?ch v?.
builder.Services.AddSingleton(jwtConfig);
// ??ng k� AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 

// Add services to the container.
builder.Services.AddMvc();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); // ??ng k� d?ch v? x�c th?c

builder.Services.AddDbContext<EmployeeDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // X�c th?c m?c ??nh 
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Th�ch th?c m?c ??nh
})
 .AddJwtBearer(options => // Th�m x�c th?c JWT
 {
     options.TokenValidationParameters = new TokenValidationParameters() // C?u h�nh c�c tham s? ki?m tra token
     {
         ValidateActor = true,
         ValidateIssuer = true,
         ValidateAudience = true,
         RequireExpirationTime = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtConfig.Issuer,
         ValidAudience = jwtConfig.Audience,
         ClockSkew = TimeSpan.Zero,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
     };

     options.Events = new JwtBearerEvents() // C?u h�nh s? ki?n x�c th?c
     {
         OnMessageReceived = context =>
         {            
             var accessToken = context.Request.Cookies["AccessToken"];
             context.Token = accessToken;
             return Task.CompletedTask;
         }
     };
 });

//Th�m d?ch v? Identity v�o d?ch v?.
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

// ??c c?u h�nh Jwt t? appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt"); 

// ??ng k� d?ch v? x�c th?c

builder.Services.AddSingleton<IAuthorizationHandler, AdminRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.Requirements.Add(new AdminRequirement()));
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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.SeedDataAsync(); // Seed d? li?u

app.Run();
