using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseraCapstone.Data;
using CourseraCapstone.Models;
using CourseraCapstone.Security;
using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CourseraCapstone
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
             _config.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 3;
                options.SignIn.RequireConfirmedEmail = true;

                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<CustomEmailConfirmationTokenProvider
            <ApplicationUser>>("CustomEmailConfirmation");

            // Changes token lifespan of all token types
            services.Configure<DataProtectionTokenProviderOptions>(o =>
                    o.TokenLifespan = TimeSpan.FromHours(5));

            // Changes token lifespan of just the Email Confirmation Token type
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
                    o.TokenLifespan = TimeSpan.FromDays(3));

            var emailConfig = _config
              .GetSection("EmailConfiguration")
              .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();

            var myConfig = _config.GetSection("myConfiguration").Get<MyConfiguration>();
            services.AddSingleton(myConfig);

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.LoginPath = new PathString("/Account/Login");
                options.LogoutPath = new PathString("/Account/Logout");
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.SlidingExpiration = true;
            });

            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddScoped<IMsgRepository, MysqlMsgRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
