using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConvenienceMVCContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConvenienceMVCContext") ??
    throw new InvalidOperationException("Connection string 'ConvenienceMVCContext' not found.")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

// セッションの追加
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Menus}/{action=Index}/{id?}");

app.Run();
