 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GeneralSalesDb.Data;
using GeneralSalesDb.Models;
using GeneralSalesDb.Services;
using System;


namespace GeneralSalesDb
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(op =>
            {
                op.SignIn.RequireConfirmedEmail = true;
                op.Lockout.MaxFailedAccessAttempts = 6;
                op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();


            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<SettingOptions>(op => Configuration.Bind(nameof(SettingOptions), op));

            //services.ConfigureAll<IISOptions>();
            //services.Configure<IdentityOptions>(op =>
            //{
            //    op.Password.RequiredLength = 4;
            //});
            services.AddAuthorization(op =>
            {
                op.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                op.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role"));
                op.AddPolicy("InsertRolePolicy", policy => policy.RequireClaim("Insert Role"));
                op.AddPolicy("ViewRolePolicy", policy => policy.RequireClaim("View Role"));
            });

            services.AddMvc(op =>
            {
                op.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                op.Filters.Add(new AuthorizeFilter(policy));
            })/*.AddJsonOptions(option=>option.SerializerSettings.ContractResolver = new DefaultContractResolver())*/;
            
            services.AddDistributedMemoryCache();
            //services.AddSession(option=> {
            //    option.IdleTimeout = TimeSpan.FromDays(30);
            //    option.Cookie.HttpOnly = true;
            //    option.Cookie.IsEssential = true;
               
            //});
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.CheckConsentNeeded = context => false;
            //    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.


        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            //app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
