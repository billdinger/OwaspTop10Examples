using Audit.Core;
using Blog.Cryptography;
using Blog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Blog.Models;

namespace Blog
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // add our custom services we need.
            services.AddSingleton<ICryptographicService, CryptographicService>();

            // A1 - Injection - Note here we're using in memory databases to save some setup time, if you'd like you can swap these to use real
            // sql servers instead for more applicable real world tests. In that case, replace the optons.UseInMemoryDatabase with
            // options.UseSqlServer(Configuration.GetConnectionString("UserContext"))); for each context you want to use and then
            // update the connection strings in appsettings.json as appropriate.

            services.AddDbContext<UserContext>(options =>
                    options.UseInMemoryDatabase("UserContext"));

            services.AddDbContext<BlogEntryContext>(options =>
                    options.UseInMemoryDatabase("BlogEntryContext"));

            services.AddDbContext<CommentContext>(options =>
                    options.UseInMemoryDatabase("CommentContext"));

            services.AddDbContext<FeedContext>(options =>
                options.UseInMemoryDatabase("FeedContext"));
        }


        /// <summary>
        /// A6 - Security Misconfiguration - This method can accidentally introduce a security flaw. The env.IsDevelopment sets us to use
        /// Developer Exception page which will link stack trace information to the browser window.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // A10 - Logging & Auditing - adding log4net here to properly log out information.
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddLog4Net();
            app.UseDeveloperExceptionPage();

            // A6 - incorrect
            app.UseDeveloperExceptionPage();

            // A6 - correct
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseBrowserLink();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/error");
            //}

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=BlogEntries}/{action=Index}/{id?}");
            });

            // A10 - Logging & Audit - Setup Audit to use log4net.
            Audit.Core.Configuration.Setup()
                .UseLog4net();
        }

    }
}
