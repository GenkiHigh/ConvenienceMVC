using ConvenienceMVC_Context;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDbContext<ConvenienceMVCContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("ConvenienceMVCContext") ??
        throw new InvalidOperationException("Connection string 'ConvenienceMVCContext' not found.")));

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    // Add services to the container.
    builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // セッションの有効期限を30分に設定
        options.Cookie.HttpOnly = true; // クッキーをHTTP通信に限定
        options.Cookie.IsEssential = true; // GDPR対応
    });

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
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
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}
