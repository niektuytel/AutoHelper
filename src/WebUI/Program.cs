using AutoHelper.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add service
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices(builder.Configuration);

var app = builder.Build();

// Use service
app.UseWebUIServices();

app.Run();
