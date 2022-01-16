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
using Noteapp.Api.Filters;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using Noteapp.Infrastructure;
using Noteapp.Infrastructure.Data;
using Noteapp.Infrastructure.Identity;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ServiceExceptionFilter>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Noteapp.Api", Version = "v1" });
            });

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("App"));
            });

            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("Identity"));
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

            // adding core services
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<INoteRepository, EfNoteRepository>();
            services.AddScoped<IAppUserRepository, EfAppUserRepository>();
            services.AddScoped<NoteService>();
            services.AddScoped<AppUserService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();

            services.Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Configuration["WebBaseUrl"].TrimEnd('/'));
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });

            services.AddAuthentication(configure =>
            {
                configure.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                configure.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var jwtSettings = Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            jwtSettings.Key)),
                    };
                });
        }

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
