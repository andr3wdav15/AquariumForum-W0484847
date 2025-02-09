using AquariumForum.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AquariumForumContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AquariumForumContext")
            ?? throw new InvalidOperationException(
                "Connection string 'AquariumForumContext' not found."
            )
    )
);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
