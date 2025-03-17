using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
//using Serilog;
//using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

//ILogger logger = SetupLogger();
//logger.LogInformation("# WAVEFORM API - REST - Start #");

var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
var resource = ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName);

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithMetrics(metrics =>

        metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        .AddMeter("System.Net.Http")
        .AddMeter("System.Net.NameResolution")
        .AddConsoleExporter()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(tracingOtlpEndpoint);
            options.Protocol = OtlpExportProtocol.Grpc;
        })
        .AddPrometheusExporter()
    )
    .WithTracing(tracing =>
    {
        tracing
        .AddSource("RestAPI.WeatherForecase")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
        });
    });

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(logging =>
{
    logging
    .SetResourceBuilder(resource)
    .AddConsoleExporter()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri(tracingOtlpEndpoint);
        options.Protocol = OtlpExportProtocol.Grpc;
    });
});
//// Configure Serilog
//Log.Logger = new LoggerConfiguration()
//    .Enrich.FromLogContext()
//    //.Enrich.WithThreadId()
//    //.Enrich.WithMachineName()
//    .WriteTo.Console()
//    .WriteTo.GrafanaLoki("http://loki:3100", // Loki URL
//        labels: new[] { new LokiLabel() {
//            Key = "app",
//            Value = "my-dotnet-api"
//        } })
//    .CreateLogger();

//// Add Serilog to the app
//builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

//app.UseSerilogRequestLogging();
//logger.LogInformation($"Environment -- {app.Environment}");
app.MapHealthChecks("/health");
app.MapGet("/", () =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Processing request for / endpoint"); // Example log
    return "Hello, OTel Collector & Console!";
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapPrometheusScrapingEndpoint();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseRouting();
app.UseEndpoints(endpoints => app.MapControllers());

app.Run();

//ILogger SetupLogger()
//{
//    using var loggerFactory = LoggerFactory.Create(builder =>
//    {
//        builder
//        .AddFilter("Microsoft", LogLevel.Warning)
//        .AddFilter("System", LogLevel.Warning)
//        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
//        .AddConsole();
//    });

//    ILogger logger = loggerFactory.CreateLogger<Program>();
//    return logger;
//}