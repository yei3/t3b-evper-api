using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Evaluation.API.Helpers;
using Evaluation.API.Services;
using Evaluation.API.Extensions;
using Evaluation.API.Models;

namespace Evaluation.API
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
            // setting CosmosDB
            services.AddDbContext<TodoContext>(opt => opt.UseCosmosSql(
                "https://bbb-yei-dev.documents.azure.com:443/",
                "ESMqKIBocTVI4ezv6OKApmKZU3S5WxwZAxt8BVgjyZ1f02BezTD3EWY2jNCu4tmt03cukfFDGJxkxscH34hMNQ==",
                "ToDoList"
            ));

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            // Add Services
            services.AddCors();
            services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDB"));            
            services.AddAutoMapper();            
            services.AddJWTAuthetication(appSettingsSection);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
