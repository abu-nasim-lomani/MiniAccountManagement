using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniAccountManagement.Data;
using MiniAccountManagement.Repositories;
using MiniAccountManagement.Repositories.Interfaces;

// --- 1. Application Builder Setup ---
var builder = WebApplication.CreateBuilder(args);

// --- 2. Database and EF Core Configuration ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Provides helpful database error pages during development.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- 3. ASP.NET Core Identity Configuration ---
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true; // Crucial for our admin approval workflow

    // Password settings - You can enable/disable these rules as needed.
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddRoles<IdentityRole>() // Enables Role Management (RoleManager)
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configures the security stamp to immediately invalidate old cookies on role/password change.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});


// --- 4. Application Services (Dependency Injection) ---

// Registering our custom repositories.
builder.Services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

// Adds services for Razor Pages.
builder.Services.AddRazorPages();


// --- 5. Build the Application ---
var app = builder.Build();


// --- 6. HTTP Request Pipeline Configuration (Middleware) ---
// The order of these middleware components is important.

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication & Authorization middleware must be between UseRouting and MapRazorPages.
app.UseAuthorization();

app.MapRazorPages();


// --- 7. Custom Application Initializer ---
// This custom method seeds the database with initial roles when the application starts.
await app.SeedRolesAsync();


// --- 8. Run the Application ---
app.Run();