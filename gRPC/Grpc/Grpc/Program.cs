using Grpc;
using Grpc.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var logger = SetupLogger();
logger.LogInformation("-- waveform grpc api started --");

var version = Environment.GetEnvironmentVariable("VERSION_NUMBER");
logger.LogInformation("Environment variable -- ", GetAssemblyVersion());

builder.Configuration.AddJsonFile("appsettings.json", false, true);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.Interceptors.Add<ServerLoggerInterceptor>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOriginDebug", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
    options.AddPolicy("AllowAll", builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors();
app.UseGrpcWeb();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
});
//app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

ILogger SetupLogger()
{
    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
    });

    ILogger logger = loggerFactory.CreateLogger<Program>();
    return logger;
}

string GetAssemblyVersion()
{
    AssemblyInformationalVersionAttribute infoVersion = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault();
    return infoVersion.InformationalVersion;
}
