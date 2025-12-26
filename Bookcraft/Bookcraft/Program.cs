using Core.Config;
using Core.DB;
using Core.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("BookSphereProject")));

builder.Services.AddSingleton<IEmailConfig>(builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfig>());
builder.Services.AddSingleton<IGeneralConfiguration>(builder.Configuration.GetSection("GeneralConfiguration").Get<GeneralConfiguration>());
//builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("BookSphereHangFire")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(43800);
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<IEmailService, EmailService>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
//UpdateDatabase(app);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

//static void UpdateDatabase(IApplicationBuilder app)
//{
//    using (var serviceScope = app.ApplicationServices
//        .GetRequiredService<IServiceScopeFactory>()
//        .CreateScope())
//    {
//        using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
//        {
//            context.Database.Migrate();
//        }
//    }
//}