using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rent.Application.Interfaces.Rent.Application.Interfaces;
using Rent.Application.Interfaces;
using Rent.Application.Services;
using Rent.Infrastructure.Data;
using Rent.Infrastructure.Repository;
using Rent.WebApi.Filters;
using Rent.WebApi.Jwt;
using Rent.WebApi.Middleware;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // Database context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Repository ve Service kayýtlarý
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ICarAvailabilityChecker, CarAvailabilityChecker>();

      
        services.AddSingleton<JwtHelper>();

        services.AddDataProtection();

        services.AddScoped<ValidationFilter>(); // ValidationFilter'ý ekle
        services.AddScoped<TimeControllerFilter>(); // TimeControllerFilter'ý ekle

        // Configure Swagger/OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Name = "Jwt Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Use the following Bearer token to authenticate with the API. You can get the token by logging in.",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                    ValidateIssuerSigningKey = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Headers.Add("Authentication-Failed", context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // HTTP istek hattýný yapýlandýr
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<JwtMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>(); // RequestLoggingMiddleware'i ekle

        app.UseRouting(); // UseRouting'in UseAuthorization'dan önce çaðrýldýðýndan emin olun

        app.UseAuthorization(); // UseAuthorization'ý buraya taþýyýn

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }


}
