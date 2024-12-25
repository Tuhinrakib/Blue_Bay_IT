using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project__Mini_Stack_Overflow.Areas.Identity.Data;
using Project__Mini_Stack_Overflow.Models;

var builder = WebApplication.CreateBuilder(args);

// Retrieve connection strings from configuration.
var applicationDbContextConnectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

var questionDbContextConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add ApplicationDbContext (for Identity) to the service container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(applicationDbContextConnectionString));

// Add default identity services with ApplicationUser and ApplicationDbContext.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Set authentication options such as requiring confirmed email for sign-in (optional).
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Store Identity data in the ApplicationDbContext.


// Add custom QuestionDbContext for application-specific models (questions, votes, etc.).
builder.Services.AddDbContext<QuestionDbContext>(options =>
    options.UseSqlServer(questionDbContextConnectionString)); // Use SQL Server for QuestionDbContext.


// Add MVC controllers and views (essential for web applications).
builder.Services.AddControllersWithViews();

// Add services for identity (login, registration, etc.)
builder.Services.AddRazorPages(); // Necessary for Identity-related pages (login, register, etc.).

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // In production, use exception handler and HSTS.
    app.UseExceptionHandler("/Home/Error"); // Custom error page for non-development environments.
    app.UseHsts(); // Strict HTTP security header to enforce HTTPS.
}

// Enable HTTPS redirection for all incoming HTTP requests.
app.UseHttpsRedirection(); // Automatically redirect HTTP to HTTPS.

app.UseStaticFiles(); // Serve static files like CSS, JavaScript, and images.

app.UseRouting(); // Enable routing for incoming requests.

app.UseAuthentication(); // Use authentication middleware (Identity framework).
app.UseAuthorization(); // Use authorization middleware (checks if user is authorized).

// Map the default route. This is the URL structure for the application.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Default routing pattern (Home/Index).

app.MapRazorPages(); // Map Razor Pages for Identity (e.g., login, register, etc.).

app.Run(); // Start the application.
