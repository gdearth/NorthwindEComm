using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.OpenApi.Models;
using NorthWindsEComm.Categories.Api;
using NorthWindsEComm.CrudHelper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
}).AddApiExplorer(config =>
{
    config.GroupNameFormat = "'v'VVV";
    config.SubstituteApiVersionInUrl = true;
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = $"Categories v1", Version = "v1" });
    var filePath = Path.Combine(AppContext.BaseDirectory, $"NorthWindsEComm.Categories.Api.xml");
    options.IncludeXmlComments(filePath);
});

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<NorthWindsDbContext>("northWindsData");

builder.AddRedisClient("cache");

builder.Services.AddTransient<ICrudManager<Category>, CrudManager<Category>>();
builder.Services.AddTransient<ICrudCacheAccess<Category>, RedisCacheHelper<Category>>();
builder.Services.AddTransient<ICrudDataAccess<Category>, CategoryDataAccess>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = $"Categories {description.GroupName}";
            options.SwaggerEndpoint(url, name);
        }
    });
}

ApiVersionSet apiVersionSet = app.NewApiVersionSet("categories-api")
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

app.UseHttpsRedirection();

var productsGroup = app.MapGroup("/v{version:apiVersion}/categories")
    .WithApiVersionSet(apiVersionSet)
    .WithOpenApi();

var productsGroupV1 = productsGroup.MapGroup("/")
    .MapToApiVersion(1);

productsGroupV1.MapGet("/",
        async (ICrudManager<Category> manager, CancellationToken token) => await manager.GetAllAsync(token))
    .WithName("GetCategories");

productsGroupV1.MapGet("/{id:int}", async (int id, ICrudManager<Category> manager, CancellationToken token) =>
    {
        Category? product = await manager.GetByIdAsync(id, token);
        return product == null ? Results.NotFound() : Results.Ok(product);
    })
    .WithName("GetCategoryById");

app.Run();