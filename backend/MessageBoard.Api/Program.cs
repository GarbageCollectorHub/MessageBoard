using MessageBoard.Api.Configuration;
using MessageBoard.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

// API services
builder.Services.AddControllers();
builder.Services.AddApiDocumentation();

// Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseApiDocumentation();
app.UseAuthorization();

app.MapControllers();

app.Run();