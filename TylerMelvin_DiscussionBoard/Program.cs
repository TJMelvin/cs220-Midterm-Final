using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TylerMelvin_DiscussionBoard.Authorization;
using TylerMelvin_DiscussionBoard.Data;
using TylerMelvin_DiscussionBoard.Helpers;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;
using TylerMelvin_DiscussionBoard.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Environment.ApplicationName = "TylerMelvin_DiscussionBoard";

string logPath = builder.Environment.ContentRootPath + "/DiscussionBoard";

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.SQLite(
        sqliteDbPath: logPath + @"-logs.db",
        restrictedToMinimumLevel: LogEventLevel.Information,
        storeTimestampInUtc: true
     )
);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment())
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyTypes.IsOwnerOrAdmin, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new IsOwnerOrAdminRequirement());
    });
});

builder.Services.AddHttpContextAccessor();


// Register repositories
builder.Services.AddScoped(typeof(IRepo<>), typeof(RepoBase<>));
builder.Services.AddScoped<IRepo<Post>, PostRepo>();
builder.Services.AddScoped<IRepo<DiscussionThread>, DiscussionThreadRepo>();

// Services
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<DiscussionThreadService>();
builder.Services.AddScoped<ApplicationUserService>();

builder.Services.AddScoped<IAuthorizationHandler, IsOwnerOrAdminHandler>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();