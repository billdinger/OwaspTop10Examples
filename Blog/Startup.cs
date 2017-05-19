using Blog.Cryptography;
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
            services.AddDbContext<UserContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("UserContext")));

            services.AddDbContext<BlogEntryContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("BlogEntryContext")));

            services.AddDbContext<CommentContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CommentContext")));
        }


        /// <summary>
        /// A5 SECURITY This method can accidentally introduce a security flaw. The env.IsDevelopment sets us to use
        /// Developer Exception page which will link stack trace information to the browser window.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            app.UseDeveloperExceptionPage();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=BlogEntries}/{action=Index}/{id?}");
            });
        }
    }
}
