using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TODO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TODO.Services;

namespace TODO
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
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<TodoContext>(options =>
            {
                options.UseSqlServer(connection);
            });
            services.AddControllers();
            services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience=AuthOptions.AuthOptions.AUDIENCE,
                        ValidateLifetime=true,
                        IssuerSigningKey=AuthOptions.AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey=true


                    };
                });
            services.AddScoped<AuthenticationService>();
            services.AddScoped<UserService>();
            services.AddScoped<CustomListService>();
            services.AddSingleton<TaskManager>();
            services.AddSingleton<ImportanceManager>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseHttpsRedirection();
            

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseStaticFiles();

        }
    }
}
