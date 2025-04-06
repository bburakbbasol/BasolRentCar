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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository ve Service kayýtlarý
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ICarAvailabilityChecker, CarAvailabilityChecker>();

// Add JwtHelper
builder.Services.AddSingleton<JwtHelper>();

// Add Data Protection
builder.Services.AddDataProtection();

// Add Filters
builder.Services.AddScoped<ValidationFilter>(); // ValidationFilter'ý ekle
builder.Services.AddScoped<TimeControllerFilter>(); // TimeControllerFilter'ý ekle

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<JwtMiddleware>(); // JwtMiddleware'i ekle
app.UseMiddleware<RequestLoggingMiddleware>(); // RequestLoggingMiddleware'i ekle

app.MapControllers();

app.Run();

