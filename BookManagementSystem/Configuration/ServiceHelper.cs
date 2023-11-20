using BookManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using StackExchange.Profiling.Storage;
using System.Text;

namespace BookManagementSystem.Configuration
{
	public static class ServiceHelper
	{
		public static IServiceCollection ServiceHelpers(this IServiceCollection services, IConfiguration configuration)
		{


			services.AddMemoryCache();
			services.AddMiniProfiler(options =>
			{
				options.RouteBasePath = "/profiler";
				options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
				options.Storage = new SqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
				options.IgnoredPaths.Add("/css");
				options.IgnoredPaths.Add("/js");
				options.IgnoredPaths.Add("/index.html");
				options.TrackConnectionOpenClose = true;// (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
														// (defaults to true, and connection opening/closing is tracked)

				options.TrackConnectionOpenClose = false;
				options.ShouldProfile = request => BookManagementSystem.Service.Function.Helper.ShouldProfile(request);
			}).AddEntityFramework();
			services.AddControllers();






			var storage = new SqlServerStorage(configuration.GetConnectionString("DefaultConnection"));


			foreach (var cs in storage.TableCreationScripts)
			{
				Console.WriteLine(cs);
			}




			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

			}).AddJwtBearer(o =>
			{
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = configuration["Jwt:Issuer"],
					ValidAudience = configuration["Jwt:Issuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
				};
			});

			services.AddAuthentication().AddCookie()
				.AddFacebook(facebookOptions =>
			{
				facebookOptions.AppId = configuration["FacebookAuthencation:AppId"];
				facebookOptions.AppSecret = configuration["FacebookAuthencation:AppSecret"];


			}).AddGoogle(googleOptions =>
			{
				googleOptions.ClientId = configuration["GoogleAuthencation:ClientId"];
				googleOptions.ClientSecret = configuration["GoogleAuthencation:ClientSecret"];
			});


			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder.AllowAnyOrigin()
						   .AllowAnyHeader()
						   .AllowAnyMethod();
				});
			});



			services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
			services.AddControllers();
			services.AddOpenApiDocument(document =>
			{
				document.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
				{
					Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
					Name = "Authorization",
					In = NSwag.OpenApiSecurityApiKeyLocation.Header,
					Description = "Type into the TextBox : Bearer {your JWT Token}"
				});
				document.OperationProcessors.Add(
					new AspNetCoreOperationSecurityScopeProcessor("JWT"));
			});


			return services;
		}
	}
}
