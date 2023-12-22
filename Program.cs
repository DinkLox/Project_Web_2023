using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using static WEB_2023.Config.SystemRules;
using WEB_2023.Areas.Admin.Services;
using WEB_2023.Data;
using WEB_2023.Entities;
using WEB_2023.Services;
using WEB_2023.Middleware;
using WEB_2023.Config;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RemoteDB") ?? throw new InvalidOperationException("Connection string not found.");
// Add services to connect to database
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString, x => x.UseDateOnlyTimeOnly()));
// Add services to configure mail settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Add services to authentication
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.SignIn.RequireConfirmedAccount = true; })
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
options.TokenLifespan = TimeSpan.FromHours(12));
// Add services to configure cookie 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/error/403";
});
// Add services to configure lockout settings
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});
// Add services to the container.

// Chỗ này đừng xóa của em!!!
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
// Add application services.
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUsersManagerService, UsersManagerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoriesManagerService, CategoriesManagerService>();
builder.Services.AddScoped<IProductManagerService, ProductManagerService>();
builder.Services.AddScoped<IBlogsManagerService, BlogsManagerService>();
// Add service to compile sass
#if DEBUG
builder.Services.AddSassCompiler();
#endif
// Add services to lower case url
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDateOnlyTimeOnlyStringConverters();
// Add services to access HttpContext from custom service
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
var options = new RewriteOptions().Add(new RedirectLowerCaseRule());
app.UseRewriter(options);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ForceSignOutOnLockout>();
app.MapControllerRoute(
    name: "areas",
    pattern: "{Area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}/{slug?}"
    );
app.Run();

