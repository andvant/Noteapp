using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using Noteapp.Infrastructure;
using Noteapp.Infrastructure.Data;
using Noteapp.Infrastructure.Identity;
using System;
using System.Text;

namespace Noteapp.Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Noteapp.Api", Version = "v1" });
            });

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.LogTo(Console.WriteLine);
                options.UseInMemoryDatabase("ApplicationDb");
            });

            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseInMemoryDatabase("IdentityDb");
            });

            services.AddIdentity<AppUserIdentity, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<IdentityContext>();

            // Singleton because it uses in-memory data which should be the same between different calls
            services.AddSingleton<INoteRepository, NoteRepository>();
            services.AddSingleton<IAppUserRepository, AppUserRepository>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IRepository<Note>, EfRepository<Note>>();
            services.AddTransient<IRepository<AppUser>, EfRepository<AppUser>>();
            services.AddTransient<NoteService>();
            services.AddTransient<AppUserService>();
            services.AddTransient<UserService>();
            services.AddTransient<TokenService>();

            services.AddCors(setup =>
            {
                setup.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });
            });

            services.AddAuthentication(configure =>
            {
                configure.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                configure.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidIssuer = "NoteappIssuer",
                        ValidateIssuer = true,
                        ValidAudience = "NoteappAudience",
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey123")),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromSeconds(5)
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Noteapp.Api v1"));
            }

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
