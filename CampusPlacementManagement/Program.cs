
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Repositories.Implementations;
using CampusPlacementManagement.Services.Interfaces;
using CampusPlacementManagement.Services.Implementations;

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
    // Email must be unique

    options.User.RequireUniqueEmail = true;

    // Password requirements

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

// 4. Configure authorization services

builder.Services.AddAuthorization();

// 5. Register controllers

builder.Services.AddControllers();

// 6. Configure OpenAPI

builder.Services.AddOpenApi();

builder.Services.AddScoped<ICollegeRepository, CollegeRepository>();

builder.Services.AddScoped<ICollegeService, CollegeService>();

var app = builder.Build();

// 7. Configure HTTP pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 8. Seed Identity roles and SuperAdmin

await DbSeeder.SeedAsync(app.Services);

app.Run();
