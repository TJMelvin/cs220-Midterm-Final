using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TylerMelvin_DiscussionBoard.Data;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;
using TylerMelvin_DiscussionBoard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment())
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// Register repositories
builder.Services.AddScoped<IRepo<Post>, PostRepo>();
builder.Services.AddScoped<IRepo<DiscussionThread>, DiscussionThreadRepo>();

// Register services
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<DiscussionThreadService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseAuthorization();

app.MapRazorPages();

app.Run();