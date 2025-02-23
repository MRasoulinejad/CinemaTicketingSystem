using CinemaTicketingSystem.Application.Common.Interfaces;
using CinemaTicketingSystem.Application.ExternalServices;
using CinemaTicketingSystem.Application.Services.Implementation;
using CinemaTicketingSystem.Application.Services.Interfaces;
using CinemaTicketingSystem.Application.Services.Interfaces.Payments;
using CinemaTicketingSystem.Application.Utility;
using CinemaTicketingSystem.Domain.Entities;
using CinemaTicketingSystem.Infrastructure.Data;
using CinemaTicketingSystem.Infrastructure.Repository;
using CinemaTicketingSystem.Infrastructure.Services;
using CinemaTicketingSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;
using AccountService = CinemaTicketingSystem.Infrastructure.Services.AccountService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register the repository
builder.Services.AddScoped<ITemporaryReservationRepository, TemporaryReservationRepository>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient();

// Register the background service
builder.Services.AddHostedService<TemporarySeatReservationCleanupService>();

// Register the ReCaptcha Validator
builder.Services.AddTransient<IReCaptchaValidator, GoogleReCaptchaValidator>();

// Register the SMTP Email Service
builder.Services.AddTransient<ISmtpEmailService, SMTPEmailService>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied"; // Show custom access denied page
    options.Events.OnValidatePrincipal = async context =>
    {
        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();

        var user = await userManager.GetUserAsync(context.Principal);
        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = context.Principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            if (!roles.SequenceEqual(claims)) // If roles changed, force logout
            {
                context.RejectPrincipal();
                await signInManager.SignOutAsync();
            }
        }
    };
});


builder.Services.Configure<SiteSettings>(
    builder.Configuration.GetSection("SiteSettings"));



builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<ITheatreService, TheatreService>();
builder.Services.AddScoped<IShowTimeService, ShowTimeService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddSingleton<IAppEnvironment, AppEnvironment>();
builder.Services.AddScoped<IStripeService, StripeService>();



var app = builder.Build();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

SeedDatabase();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

void SeedDatabase()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}