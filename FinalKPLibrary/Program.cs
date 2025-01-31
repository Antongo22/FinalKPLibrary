using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly"); // Только админы могут доступть /Admin
    options.Conventions.AuthorizeFolder("/"); // Требовать авторизацию для всех страниц

    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
    options.Conventions.AllowAnonymousToPage("/Account/Logout");

});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 МБ
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // Куки доступны только через HTTP
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Время жизни кук
    options.LoginPath = "/Account/Login"; // Путь к странице входа
    options.AccessDeniedPath = "/Account/AccessDenied"; // Путь к странице "Доступ запрещён"
    options.SlidingExpiration = true; // Обновление времени жизни кук при активности
    options.Cookie.Name = "YourAppAuthCookie"; // Имя куки (опционально)
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // Создаём роль "admin", если её нет
        if (!await roleManager.RoleExistsAsync("admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<int>("admin"));
        }

        // Создаём роль "user", если её нет
        if (!await roleManager.RoleExistsAsync("user"))
        {
            await roleManager.CreateAsync(new IdentityRole<int>("user"));
        }

        // Проверяем, существует ли пользователь с логином "admin"
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = "admin",
                Type = "admin"
            };

            var result = await userManager.CreateAsync(adminUser, "12345678");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating roles or admin user.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Аутентификация должна быть первой
app.UseAuthorization();  // Затем авторизация

app.UseSession(); // Подключение сессий (если они используются)

app.MapRazorPages();
app.UseCors("AllowAll");
app.Run();

// TODO : ошибка при добавлени пустой области видимости