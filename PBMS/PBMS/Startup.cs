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
using PBMS.Data;
using PBMS.Models;
using PBMS.Services;

namespace PBMS
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    //IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");
                    //options.ClientId = googleAuthNSection["394603513818-ca367il6iiqrkmnoso2u0kqkvbr8ciaa.apps.googleusercontent.com"];
                   //options.ClientSecret = googleAuthNSection["oXgwzXigyh_ZY9vGEyphjiiq"];
                    options.ClientId = "394603513818-ca367il6iiqrkmnoso2u0kqkvbr8ciaa.apps.googleusercontent.com";
                    options.ClientSecret = "oXgwzXigyh_ZY9vGEyphjiiq";
                    //options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v1/certs";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "296591604668015";
                    options.AppSecret = "7655fa2414c75058dd17528be38e3be6";
                });

            services.AddDistributedMemoryCache();

            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(15);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //DBInitializer.Initialize(context, userManager, roleManager).Wait();
        }
    }
}
