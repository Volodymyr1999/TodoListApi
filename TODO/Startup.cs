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
using System.Security.Cryptography.Xml;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;

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

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("TodoListOpenApiSpecification", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title="TodoList",
                    Version="1",
                    
                });
                setupAction.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description=@"Jwt authorization header using the Bearer scheme\r\n\r\n
                    Enter 'Bearer' [space] and then your token in the textinput below\r\n\r\Example 'Bearer 1234wgwugweug'",
                    Name="Authorization",
                    In=Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type=Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme="Bearer"
                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                  });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommetntsPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                setupAction.IncludeXmlComments(xmlCommetntsPath);

            });
            
        }

        ///<remarks>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</remarks> 
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
            app.UseSwagger();
            
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/TodoListOpenApiSpecification/swagger.json", "TodoList");
            });
            app.UseStaticFiles();

        }
    }
}
