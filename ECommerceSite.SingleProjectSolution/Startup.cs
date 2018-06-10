using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ECommerceSite.SingleProjectSolution.Data;
using ECommerceSite.SingleProjectSolution.Models;
using ECommerceSite.SingleProjectSolution.Services;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

namespace ECommerceSite.SingleProjectSolution
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRoles>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();


            var dirPath = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", "Uploads");


            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dirPath);
            }


            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                   dirPath));


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

           
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            var options = new DefaultFilesOptions
            {
                RequestPath = new PathString("/wwwroot/Uploads")
            };
            app.UseDefaultFiles(options);
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            ApplicationDbContext.CreateAdminAccount(app.ApplicationServices,
       Configuration).Wait();
        }
    }
}
