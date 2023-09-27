using BookManagementSystem;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using Serilog;
using StackExchange.Profiling;
using StackExchange.Profiling.Storage;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
		.ReadFrom.Configuration(builder.Configuration)
		.Enrich.FromLogContext()
		.CreateLogger();

builder.Services.AddMemoryCache();
//builder.Services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>();
builder.Services.AddMiniProfiler(options =>
{
	options.RouteBasePath = "/profiler";
	options.Storage = new SqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
	options.IgnoredPaths.Add("/css");
	options.IgnoredPaths.Add("/js");
	options.IgnoredPaths.Add("/index.html");
	options.ShouldProfile = request => request.Path.StartsWithSegments("/api");
	options.TrackConnectionOpenClose = false;
}).AddEntityFramework();
builder.Services.AddControllers();




var storage = new SqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));


foreach (var cs in storage.TableCreationScripts)
{
	Console.WriteLine(cs);
}


builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.InfrastructureServices(builder.Configuration);
builder.Services.ServiceServices(builder.Configuration);

builder.Services.AddAuthorization();


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Issuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(document =>
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



var app = builder.Build();
app.UseOpenApi();

app.UseSerilogRequestLogging();
app.UseMiniProfiler();

app.UseSwaggerUi3(x =>
	{
		x.Path = "/swagger";
	});

app.UseMiddleware<ErrorHandlerMiddleWare>();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
