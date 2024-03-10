using System.Text.Json.Serialization;
using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.WebApi.Configuration;
using InDuckTor.Credit.WebApi.Endpoints;
using InDuckTor.Shared.Configuration;
using InDuckTor.Shared.Security;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddProblemDetails().ConfigureJsonConverters();
builder.Services.Configure<JsonOptions>(cfg =>
{
    var enumMemberConverter = new JsonStringEnumMemberConverter(
        new JsonStringEnumMemberConverterOptions(),
        typeof(ApplicationState));
    cfg.SerializerOptions.Converters.Add(enumMemberConverter);
});

builder.Services.AddInDuckTorSecurity();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddHangfire(cfg => cfg
//     .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
//     .UseSimpleAssemblyNameTypeSerializer()
//     .UseRecommendedSerializerSettings()
//     .UsePostgreSqlStorage(configure =>
//     {
//         // configure.UseNpgsqlConnection();
//     })
// );

// Add the processing server as IHostedService
// builder.Services.AddHangfireServer();

builder.Services.AddSwaggerGen(cfg =>
{
    cfg.ConfigureJwtAuth();
    cfg.ConfigureEnumMemberValues();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddLoanApplicationEndpoints()
    .AddLoanProgramEndpoints()
    .AddLoanEndpoints();

app.UseHttpsRedirection();

app.Run();