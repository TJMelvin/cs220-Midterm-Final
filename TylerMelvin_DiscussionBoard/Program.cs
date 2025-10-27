using Microsoft.EntityFrameworkCore;
using TylerMelvin_DiscussionBoard.Data;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;
using TylerMelvin_DiscussionBoard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment())
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register repositories
builder.Services.AddScoped(typeof(IRepo<>), typeof(RepoBase<>));
builder.Services.AddScoped<IRepo<Post>, PostRepo>();
builder.Services.AddScoped<IRepo<DiscussionThread>, DiscussionThreadRepo>();

// Services
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<DiscussionThreadService>();


builder.Services.AddRazorPages();

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