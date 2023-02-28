using Flight.WEBAPP.Common;
using Flight.WEBAPP.Common.Interfaces;
using Flight.WEBAPP.Services.TOOLS;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICachingHelper, CachingHelper>();
builder.Services.AddScoped<IResponseProvider, ResponseProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
