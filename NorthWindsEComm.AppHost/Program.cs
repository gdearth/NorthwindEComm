using System.Reflection;
using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", true);

var sql = builder.AddSqlServer("northWindsDb", password)
    .WithBindMount("./sqlserverconfig", "/usr/config")
    .WithBindMount("./sqlserverscripts", "/docker-entrypoint-initdb.d")
    .WithEntrypoint("/usr/config/entrypoint.sh").AddDatabase("northWindsData", "NorthWinds");

var redis = builder.AddRedis("cache");

var kafka = builder.AddKafka("messaging")
    .WithKafkaUI();

var productsApi = builder.AddProject<NorthWindsEComm_Products_Api>("products-api")
    .WithReference(sql)
    .WithReference(redis)
    .WithReference(kafka);

var suppliersApi = builder.AddProject<NorthWindsEComm_Suppliers_Api>("suppliers-api")
    .WithReference(sql)
    .WithReference(redis)
    .WithReference(kafka);

var categoriesApi = builder.AddProject<NorthWindsEComm_Categories_Api>("categories-api")
    .WithReference(sql)
    .WithReference(redis)
    .WithReference(kafka);

var gateway = builder.AddProject<NorthWindsEComm_Gateway>("gateway")
    .WithEnvironment("Services__productsApi__DownstreamPath", productsApi.GetEndpoint("https"))
    .WithEnvironment("Services__categoriesApi__DownstreamPath", categoriesApi.GetEndpoint("https"))
    .WithEnvironment("Services__suppliersApi__DownstreamPath", suppliersApi.GetEndpoint("https"))
    .WithExternalHttpEndpoints();

builder.Build().Run();
