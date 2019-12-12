using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using back_end.Data;

namespace back_end
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            List<string> allowedOrigins = new List<string>(){"https://localhost:4100", "http://localhost:4100", "https://localhost:4200", "http://localhost:4200" };
            services.AddCors(options => {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder => {
                    builder
                        .AllowAnyHeader()
                        .SetIsOriginAllowed((origin) => {
                            foreach (var item in allowedOrigins) {
                                if (origin == item) return true;
                            }
                            return false;
                        })
                        .AllowAnyMethod();
                });
            });
            
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")) );
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
