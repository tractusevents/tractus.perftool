// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.FileProviders;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(); // Add support for controllers
        services.AddDirectoryBrowser();

        services.AddLogging(l =>
        {
            if (!Program.EnableWebLogging)
            {
                l.ClearProviders();
                l.SetMinimumLevel(LogLevel.None);
            }
            else
            {
                l.SetMinimumLevel(LogLevel.Debug);
            }
        });

        services.AddCors(c =>
        {
            c.AddDefaultPolicy(p =>
            {
                p.AllowAnyHeader();
                p.AllowAnyMethod();
                p.AllowAnyOrigin();
            });
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseCors();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseFileServer(new FileServerOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csv")),
            RequestPath = "/csv",
            EnableDirectoryBrowsing = true
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Map controller endpoints
        });

        
    }
}
