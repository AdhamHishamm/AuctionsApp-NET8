using System.Security.Claims;
using Auctions.Data;
using Auctions.Data.Services;
using Auctions.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Disable account confirmation
    options.SignIn.RequireConfirmedEmail = false;   // Disable email confirmation

})
.AddRoles<IdentityRole>() //HERE IS THE ERRORRRR
.AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddRazorPages();


builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Listings/Create", "Admin");
    options.Conventions.AuthorizePage("/Listings/MyListings", "Admin");
});


builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IListingsService, ListingsService>();
builder.Services.AddScoped<ICommentsService, CommentsService>();
builder.Services.AddScoped<IBidsService, BidsService>();

builder.Services.AddHostedService<AuctionClosingService>(); //NEW

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Listings}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();