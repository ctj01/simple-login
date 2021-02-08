using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationR.Models;
using WebApplicationR.Security.Jwt;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WebApplicationR
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
            services.AddDbContext<DbContextUser>(options => options.UseSqlServer(Configuration.GetConnectionString("RolConnection")));
            services.AddMediatR(typeof(UserRequest.Handler).Assembly);
            services.AddControllersWithViews(opt =>
            {

                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            });
            var builder = services.AddIdentityCore<User>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddRoles<IdentityRole>();
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<User, IdentityRole>>();
            identityBuilder.AddEntityFrameworkStores<DbContextUser>();
            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
            }).AddDefaultTokenProviders();
            
            identityBuilder.AddSignInManager<SignInManager<User>>();
            services.TryAddSingleton<ISystemClock, SystemClock>();
            services.AddScoped<Security.Jwt.IJwtGenerator, Security.Jwt.JwtGenerator >();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("here you must write the hash for encrypt token /////////"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => opt.TokenValidationParameters
           = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = key,
               ValidateAudience = false,
               ValidateIssuer = false
           });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                               .ActionContext;
                return new UrlHelper(actionContext);
            });
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
