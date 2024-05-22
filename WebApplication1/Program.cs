using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestApi.Data;
using TestApi.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints().AddDefaultTokenProviders();
// Configure authentication to use cookies
builder.Services.AddAuthentication(
    options=>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }
)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        options.LoginPath = "/User/login";
        options.LogoutPath = "/User/logout";
        options.AccessDeniedPath = "/User/AccessDenied";
        options.SlidingExpiration = true;

    });



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle4

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"))); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configure the application cookie
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.SameSite=SameSiteMode.Strict;
//    options.Cookie.HttpOnly = true;
    
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
//    options.LoginPath = "/User/login";
//    options.LogoutPath = "/User/logout";
//    options.AccessDeniedPath = "/User/AccessDenied";
//    options.SlidingExpiration = true;
//});
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<IdentityUser>();

app.MapControllers();


app.Run();
