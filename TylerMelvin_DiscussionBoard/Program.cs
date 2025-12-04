using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TylerMelvin_DiscussionBoard.Authorization;
using TylerMelvin_DiscussionBoard.Data;
using TylerMelvin_DiscussionBoard.Helpers;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;
using TylerMelvin_DiscussionBoard.Services;

var builder = WebApplication.CreateBuilder(args);

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

using (var scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;

    try
    {
        ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

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