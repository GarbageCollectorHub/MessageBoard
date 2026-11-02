using MessageBoard.Api.Configuration;
using MessageBoard.Application;
using MessageBoard.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Services.AddApiLogging(builder.Configuration);

// API services
builder.Services.AddControllers();
builder.Services.AddApiDocumentation();
builder.Services.AddApiErrorHandling();

// Application services
builder.Services.AddApplication();

// Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseApiDocumentation();
app.UseAuthorization();

app.MapControllers();


app.Run();