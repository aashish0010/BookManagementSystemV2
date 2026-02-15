using BookManagementSystem.Configuration;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
		.ReadFrom.Configuration(builder.Configuration)
		.Enrich.FromLogContext()
		.CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);


builder.Services.ServiceHelpers(builder.Configuration);


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder => policyBuilder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            //.AllowCredentials()
            .SetIsOriginAllowed(_ => true)
    );
});

builder.Services.InfrastructureServices(builder.Configuration);
builder.Services.ServiceServices(builder.Configuration);

builder.Services.AddAuthorization();





var app = builder.Build();
app.UseOpenApi();
app.UseCors("AllowAllOrigins");


app.UseSerilogRequestLogging();
app.UseMiniProfiler();

app.UseSwaggerUi3(x =>
	{
		x.Path = "/swagger";
	});

app.UseStaticFiles();

app.UseMiddleware<ErrorHandlerMiddleWare>();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
