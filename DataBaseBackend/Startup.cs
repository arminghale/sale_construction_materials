using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseBackend
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
            services.AddDbContext<Context>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("Context")));
            services.AddScoped<IViewRenderService, RenderViewToString>();



            

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                CookieAuthenticationDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
                options.AddPolicy("JWTAdmin", policy =>
                {

                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Role, new string[] { "admin" }).Build();
                });
                options.AddPolicy("JWTAnbar", policy =>
                {

                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Role, new string[] { "anbar" }).Build();
                });
                options.AddPolicy("JWTAll", policy =>
                {

                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Role, new string[] {"user" }).Build();
                });
                options.AddPolicy("CookiesAdmin", policy =>
                {

                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Role, new string[] { "admin" }).Build();
                });
                options.AddPolicy("CookiesAnbar", policy =>
                {

                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Role, new string[] { "anbar" }).Build();
                });
                options.AddPolicy("CookiesAll", policy =>
                {

                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Role, new string[] { "user" }).Build();
                });
            });
            
            services.AddMvc()
            .AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/Views/Shared/Components/{0}/Default.cshtml");
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "cookie";
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                    options.AccessDeniedPath = "/Home/Error";
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = "https://localhost:44390",
                        ValidIssuer = "https://localhost:44390",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("azdatabasemazidi"))
                    };
                });
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(config =>
            //{
            //    config.Cookie.Name = "cookie";
            //    config.LoginPath = "/Login";
            //    config.LogoutPath = "/Logout";
            //    config.ExpireTimeSpan = TimeSpan.FromMinutes(34700);
            //});
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.SaveToken = true;
            //    // اعتبار سنجی توکن
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        // آیا سرور ایجاد کننده توکن اعتبار سنجی شود ؟ بله
            //        ValidateIssuer = true,

            //        // آیا دریافت کننده توکن اعتبار سنجی شود ؟ بله
            //        ValidateAudience = true,

            //        // آیا منقضی شدن توکن ایجاد شده بررسی شود ؟ بله
            //        ValidateLifetime = true,

            //        // آیا کلید امضا اعتبار سنجی شود ؟ بله
            //        ValidateIssuerSigningKey = true,

            //        // سرور یا ایجاد کننده معتبر
            //        ValidIssuer = "https://localhost:44390",

            //        // دریافت کننده معتبر
            //        ValidAudience = "https://localhost:44390",


            //        // نکته: اگر خواستید از چند دریافت کننده یا ایجاد کننده استفاده کنید از کد های زیر استفاده کنید

            //        // سرور یا ایجاد کننده های معتبر

            //        //ValidIssuers = new[] {"http://example.com", "http://example.com" },

            //        // دریافت کننده های معتبر


            //        //ValidAudiences = new[] { "http://example.com", "http://example.com" }

            //        // jwt کلیدی برای ایجاد امضا برای 
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("azdatabasemazidi"))
            //    };
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseDeveloperExceptionPage();
            app.UseHsts();
            //if (env.IsDevelopment())
            //{

            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            //app.UseCors(policy =>
            //{
            //    policy.AllowAnyHeader();
            //    policy.AllowAnyMethod();
            //    policy.AllowAnyOrigin();
            //    policy.AllowCredentials();
            //});

            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    var bearerAuth = context.Request.Headers["Authorization"]
            //        .FirstOrDefault()?.StartsWith("Bearer ") ?? false;
            //    if (context.Response.StatusCode == 401
            //        && !context.User.Identity.IsAuthenticated
            //        && !bearerAuth)
            //    {
            //        await context.ChallengeAsync("Cookies");
            //    }
            //});

            app.UseAuthentication();
            app.UseRouting();
            
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                        name: "Areas",
                        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
