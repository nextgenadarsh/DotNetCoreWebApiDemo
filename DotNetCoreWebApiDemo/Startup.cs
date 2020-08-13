using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreWebApiDemo.Handlers;
using DotNetCoreWebApiDemo.Models;
using DotNetCoreWebApiDemo.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly:ApiConventionType(typeof(DefaultApiConventions))] // Apply the default api conventions
namespace DotNetCoreWebApiDemo
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
            //services.AddHttpClient(); // Generic HttpClient
            services.AddHttpClient("github", c => { // Named HttpClient
                c.BaseAddress = new Uri("https://api.github.com/");
                c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v2+json");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactoryDemo");
            });

            services.AddTransient<ValidateHttpClientHandler>();
            services.AddHttpClient<GitService>(httpClient => {
                httpClient.BaseAddress = new Uri("https://api.github.com/");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v2+json");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactoryDemo");
            }).AddHttpMessageHandler<ValidateHttpClientHandler>();
            //services.AddSingleton<IGitService, GitService>();


            // Register Student Database Settings section
            services.Configure<StudentDbSettings>(Configuration.GetSection(nameof(StudentDbSettings)));

            // Register the instance of Student Database Settings
            services.AddSingleton<IStudentDbSettings>(sp => sp.GetRequiredService<IOptions<StudentDbSettings>>().Value);

            

            // Enable OAuth using Auth0 Provider
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.Authority = "https://nextgenadarsh.us.auth0.com/";
                options.Audience = "https://localhost:5001/";
            });

            // Register DbContext for StudentsDb
            services.AddDbContext<StudentContext>(opt => opt.UseInMemoryDatabase("StudentsDb"));
            //services.AddScoped<IStudentService, EfStudentService>();
            services.AddScoped<IStudentService, MongoDbStudentService>();
            services.AddDirectoryBrowser();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="antiforgery">IAntiforgery antiforgery</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error-local");
            } else
            {
                app.UseHsts(); // Adds strict transport security header
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.Use(next => ctx =>
            {
                ctx.Response.Cookies.Append("XSRF-TOKEN", "antiforgery.GetAndStoreTokens(ctx).RequestToken", new CookieOptions { HttpOnly = false});
                return next(ctx);
            });

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot-external")),
                RequestPath = "/static-external", // https://localhost:5001/static-external/
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true
            }); // UseDefaultFiles() + UseStaticFiles() + DirectoryBrowsing

            // Serve static files from custom directory and with custom path in url
            app.UseDefaultFiles();
            const long cacheMaxAge = 300;
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot")),
                RequestPath="/static",
                OnPrepareResponse = staticFileContext => {
                    // Make static file publicly cacheable for 300 Seconds
                    staticFileContext.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cacheMaxAge}");
                }
            }); // Serves the static files from wwwroot folder eg: https://localhost:5001/static/Index.html

            // Allow the user to browser the directory like https://localhost:5001/static-directory/
            app.UseDirectoryBrowser(new DirectoryBrowserOptions {
                FileProvider = new PhysicalFileProvider (Path.Combine(env.WebRootPath, "wwwroot-directory")),
                RequestPath = "/static-directory"
            });

            app.UseRouting();

            app.UseAuthentication();    // Enables authentication capabilities
            app.UseAuthorization();     // Enables authorization capabilities

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
