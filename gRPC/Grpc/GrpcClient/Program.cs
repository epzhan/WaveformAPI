using Grpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("System", LogLevel.Warning)
    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
    ;
});

ILogger logger = loggerFactory.CreateLogger<Program>();

//var config = SetupBuild();

//using var channel = GrpcChannel.ForAddress(config["appsettings:url-local"]);

//var client = new Greeter.GreeterClient(channel);
//var reply = await client.SayHelloAsync(new HelloRequest { Name = "Hello" });
//Console.WriteLine("Greeting..", reply.Message);

Console.WriteLine("Press any key to exit..");
Console.ReadLine();

IConfiguration SetupBuild()
{
    //var configuration = new ConfigurationBuilder()
    //.AddJsonFile("config.json")
    //.Build();
    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", OptionalAttribute: false);

    IConfiguration config = builder.Build();
    return config;
}

//ILogger SetupLogger()
//{
//    using var loggerFactory = LoggerFactory.Create(builder =>
//    {
//        builder.AddFilter("Microsoft", LogLevel.Warning)
//        .AddFilter("System", LogLevel.Warning)
//        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
//        .AddConsole();
//    });

//    ILogger logger = loggerFactory.CreateLogger<Program>();
//    return logger;
//}