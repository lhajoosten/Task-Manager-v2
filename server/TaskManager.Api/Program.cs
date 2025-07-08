using System.Security.Cryptography.X509Certificates;

namespace TaskManager.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Create the builder and disable automatic endpoint binding
			var builder = WebApplication.CreateBuilder(args);
			builder.WebHost.UseSetting("server.urls", null);

			X509Certificate2 certificate;
			try
			{
				// Load certificate from PEM files (already contains the private key)
				certificate = X509Certificate2.CreateFromPemFile("/https/localhost.crt", "/https/localhost.key");
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to load certificate from PEM files. Ensure /https/localhost.crt and /https/localhost.key are valid.", ex);
			}

			// Configure Kestrel manually to use the certificate
			builder.WebHost.ConfigureKestrel(options =>
			{
				options.ListenAnyIP(8443, listenOptions =>
				{
					listenOptions.UseHttps(certificate);
				});
				// Also listen on HTTP if needed
				options.ListenAnyIP(8080);
			});

			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();
			app.Run();
		}
	}
}
