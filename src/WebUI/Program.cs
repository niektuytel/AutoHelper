using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Infrastructure.Common;
using AutoHelper.Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;

CultureConfig.SetGlobalCultureToNL();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices(builder.Configuration);

var app = builder.Build();

// Use service
app.UseWebUIServices();

app.Run();
