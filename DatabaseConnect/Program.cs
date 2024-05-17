using Application;
using Application.CommandHandlers;
using Application.Commands;
using Application.Encryptions;
using Domain;
using Infrastructure;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Retrieve connection strings 
var mysqlConnectionString = "server=localhost;port=3306;user=root;password=Aiman.2001-;database=sys";
var sqlServerConnectionString = "Data Source=.\\sqlexpress;Initial Catalog=Users;Integrated Security=True";

// Service registration for the servers
builder.Services.AddScoped<Database>(_ => new SQLDatabase(sqlServerConnectionString));
builder.Services.AddScoped<Database>(_ => new MYSQLDatabase(mysqlConnectionString));

// Register specific implementations 
builder.Services.AddScoped<SQLDatabase>(_ => new SQLDatabase(sqlServerConnectionString));
builder.Services.AddScoped<MYSQLDatabase>(_ => new MYSQLDatabase(mysqlConnectionString));

// Register database implementations to use in commandHandler
builder.Services.AddSingleton<Database, SQLDatabase>(_ => new SQLDatabase(sqlServerConnectionString));
builder.Services.AddSingleton<Database, MYSQLDatabase>(_ => new MYSQLDatabase(mysqlConnectionString));

// Register MediatR manually since .NET 4.7.2 does not support mediatr.dependencyInjection
builder.Services.AddScoped<IMediator, Mediator>();

// Register the database resolver to implement the mediatr
builder.Services.AddScoped<Func<string, Database>>(serviceProvider => key =>
{
    return key switch
    {
        "mssql" => serviceProvider.GetService<SQLDatabase>(),
        "mysql" => serviceProvider.GetService<MYSQLDatabase>(),
        _ => null
    };
});

// Register the handlers
builder.Services.AddTransient<IRequestHandler<AddUserCommand, Unit>, AddUserCommandHandler>();
builder.Services.AddTransient<IRequestHandler<GetAllUsersQuery, List<UserAdd>>, GetAllUsersQueryHandler>();

// Build the service provider
var serviceProvider = builder.Services.BuildServiceProvider();

// Resolve the IMediator instance
var mediator = serviceProvider.GetService<IMediator>();

// Register Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Register encryption services
string encryptionKey = "ENC"; // Ideally, this should be securely generated or fetched
builder.Services.AddSingleton<AESEncryption>(new AESEncryption(encryptionKey));
// Register RSA encryption service
builder.Services.AddScoped<RSAEncryption>();


//var encryptionKey = "ENC"; // Replace with a secure key from configuration
//builder.Services.AddSingleton<Encrypt>(new AESEncryption(encryptionKey));
//builder.Services.AddSingleton<RSAEncryption>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();