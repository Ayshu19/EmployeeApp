using EmployeeApp.Data; // For EmployeeRepository

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the EmployeeRepository for dependency injection
builder.Services.AddScoped<EmployeeRepository>();

// (Optional) Enable session support – only if you plan to use sessions later
builder.Services.AddDistributedMemoryCache();
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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session (optional – safe to keep in for future use)
app.UseSession();

app.UseAuthorization();

// Default route: EmployeeController -> Index action
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Create}/{id?}");

app.Run();
