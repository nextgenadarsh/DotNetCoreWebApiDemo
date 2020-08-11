using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreWebApiDemo.Models;
using DotNetCoreWebApiDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddScoped<IStudentService, EfStudentService>();
            //services.AddScoped<IStudentService, MongoDbStudentService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(); // Serves the static files from wwwroot folder eg: https://localhost:5001/Index.html

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
