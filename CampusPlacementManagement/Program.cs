using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Repositories.Implementations;
using CampusPlacementManagement.Services.Interfaces;
using CampusPlacementManagement.Services.Implementations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Read SQL Server connection string

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is not configured.");

// 2. Register ApplicationDbContext

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDataProtection();

// 3. Configure ASP.NET Core Identity

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// 4. Configure JWT Authentication

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey))
            };
    });

// 5. Configure Authorization

builder.Services.AddAuthorization();

// 6. Register controllers and OpenAPI

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 7. Register College services

builder.Services.AddScoped<ICollegeRepository, CollegeRepository>();
builder.Services.AddScoped<ICollegeService, CollegeService>();

// 8. Register Placement Officer services

builder.Services.AddScoped<
    IPlacementOfficerService,
    PlacementOfficerService>();

builder.Services.AddScoped<
    IPlacementOfficerRepository,
    PlacementOfficerRepository>();

// 9. Register Authentication service

builder.Services.AddScoped<IAuthService, AuthService>();

// Company Management

builder.Services.AddScoped<
    ICompanyRepository,
    CompanyRepository>();

builder.Services.AddScoped<
    ICompanyService,
    CompanyService>();

builder.Services.AddScoped<ICompanyHRRepository, CompanyHRRepository>();

builder.Services.AddScoped<ICompanyHRService, CompanyHRService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();


var app = builder.Build();

// 10. Configure HTTP pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// 11. Seed Identity roles and SuperAdmin

await DbSeeder.SeedAsync(app.Services);

app.Run();