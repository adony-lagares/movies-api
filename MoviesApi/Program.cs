using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesApi.Data;
using MoviesApi.Repositories;
using MoviesApi.Services;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Net;
using System.Text;
using System.Text.Json;

/// <summary>
/// Entry point for the Movies API.
/// Configures logging, authentication, dependency injection, and middleware.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configures logging with Serilog and Elasticsearch.
/// </summary>
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"moviesapi-logs-{DateTime.UtcNow:yyyy-MM}",
        FailureCallback = (logEvent, exception) =>
        {
            Console.WriteLine($"Failed to send log to Elasticsearch: {exception?.Message}");
        },
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog
    })
    .CreateLogger();

builder.Host.UseSerilog();

/// <summary>
/// Configures API to listen on port 80 inside the container.
/// </summary>
builder.WebHost.UseUrls("http://+:80");

/// <summary>
/// Configures the database connection (SQL Server).
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// Configures JWT authentication.
/// </summary>
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

/// <summary>
/// Configures Dependency Injection for services and repositories.
/// </summary>
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteMovieRepository, FavoriteMovieRepository>();
builder.Services.AddScoped<IFavoriteMovieService, FavoriteMovieService>();
builder.Services.AddHttpClient<IOmdbService, OmdbService>();

/// <summary>
/// Configures Rate Limiting (100 requests per minute).
/// </summary>
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.PermitLimit = 100;
    });
});

/// <summary>
/// Configures Health Checks.
/// </summary>
builder.Services.AddHealthChecks();

/// <summary>
/// Configures Swagger/OpenAPI for API documentation.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter the JWT token as: Bearer {your_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

/// <summary>
/// Global middleware for handling application errors.
/// </summary>
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            Log.Error(exceptionHandlerPathFeature.Error, "Internal application error.");

            var errorResponse = new
            {
                Message = "An internal error occurred. Please try again later.",
                StatusCode = context.Response.StatusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    });
});

/// <summary>
/// Registers essential middlewares.
/// </summary>
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
