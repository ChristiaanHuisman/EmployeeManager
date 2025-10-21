using Microsoft.EntityFrameworkCore;
using Q3_CodeLink_EMS.Data;
using Q3_CodeLink_EMS.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Q3 - 6. Configure Program.cs
builder.Services.AddDbContext<CodeLinkEmsDbContext>(options => options.UseInMemoryDatabase("CodeLinkEmsDb")); // Adding DbContext with InMemory
builder.Services.AddScoped<EmployeeService>(); // Registering service
builder.Services.AddScoped<AdminUserService>(); // Registering service
// Serializing enum values as strings
builder.Services.AddControllers().AddJsonOptions(options => 
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddDistributedMemoryCache(); // Storing session data in memory
// Sessions are used with own designed authentication
// Identity can also be used for authentication with its own built in functions
// Identiy can be easier to use but might require a bit more overhead
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

// Q3 - 6. Configure Program.cs
app.UseSession();

app.UseAuthorization();

// Q3 - 6. Configure Program.cs
// Setting default view route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

app.Run();
