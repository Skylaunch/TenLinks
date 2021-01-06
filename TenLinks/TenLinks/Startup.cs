using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenLinks.Models;
using Microsoft.EntityFrameworkCore;

namespace TenLinks
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
            string connectionString = "Server=DESKTOP-4Q97L55\\SQLEXPRESS;Database=TenLinksDatabase;Trusted_Connection=True;";

            services.AddDbContext<LinkContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<KeywordContext>(options => options.UseSqlServer(connectionString));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Keyword}/{action=Index}");
            });
        }
    }
}
