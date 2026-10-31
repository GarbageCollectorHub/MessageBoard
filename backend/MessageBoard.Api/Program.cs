using MessageBoard.Api.Configuration;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiDocumentation();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseApiDocumentation();
app.UseAuthorization();

app.MapControllers();

app.Run();