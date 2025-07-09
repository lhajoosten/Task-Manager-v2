using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TaskManager.Api.Configuration;
using TaskManager.Api.Middleware;
using TaskManager.Application;
using TaskManager.Persistence;

namespace TaskManager.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Disable automatic endpoint binding
        builder.WebHost.UseSetting("server.urls", null);

        // Configure SSL
        ConfigureSSL(builder);

        // Add services to the container
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline
        ConfigurePipeline(app);

        app.Run();
    }

    private static void ConfigureSSL(WebApplicationBuilder builder)
    {
        X509Certificate2 certificate;
        try
        {
            certificate = X509Certificate2.CreateFromPemFile("/https/localhost.crt", "/https/localhost.key");
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load certificate from PEM files.", ex);
        }

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8443, listenOptions => listenOptions.UseHttps(certificate));
            options.ListenAnyIP(8080);
        });
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add controllers with API versioning
        services.AddControllers();
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("version"),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("ver"));
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        // Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", builder =>
            {
                builder.WithOrigins("https://localhost:6443", "http://localhost:6000")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
        });

        // Add Authentication & Authorization
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "SuperSecretKeyThatIsAtLeast32Characters!"))
                };
            });

        services.AddAuthorization();

        // Add HTTP Context Accessor
        services.AddHttpContextAccessor();

        // Add application layers
        services.AddApplicationServices();
        services.AddPersistenceServices(configuration);

        // Add Swagger
        services.AddSwaggerConfiguration();
        services.AddEndpointsApiExplorer();
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        // Exception handling
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Development-specific middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API V1");
                c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
            });
        }

        // Security headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            await next();
        });

        app.UseHttpsRedirection();
        app.UseCors("DefaultPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
