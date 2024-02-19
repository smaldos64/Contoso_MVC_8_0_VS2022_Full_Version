using Contoso_MVC_8_0_VS2022.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Contoso_MVC_8_0_VS2022
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            // LTPE ==>
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            // ==> LTPE

            // LTPE ==>
            builder.Services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            // Kræver installation af Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
            // pakke !!!
            // ==> LTPE

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // LTPE ==>
            // Ensure database creation and seeding before app starts
            EnsureDatabaseCreatedAndSeeded(app);
            // ==> LTPE

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void EnsureDatabaseCreatedAndSeeded(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SchoolContext>();
                dbContext.Database.EnsureCreated();
                DbInitializer.Initialize(dbContext);
            }
        }
    }
}
