using NorthWindsEComm.Gateway;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Hosting;
using MMLib.Ocelot.Provider.AppConfiguration;
using Ocelot.Administration;using Ocelot.Configuration.File;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
    reloadOnChange: true);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddServiceDiscovery();
builder.Services.AddDnsSrvServiceEndpointProvider();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddServiceDefaults();

builder.Services.AddHttpClient<ProductWithSupplierAggregator>();

builder.Services.AddOcelot()
    .AddAppConfiguration()
    .AddTransientDefinedAggregator<ProductWithSupplierAggregator>()
    .AddAdministration("/administration", "secret");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseOcelot().Wait();
app.Run();