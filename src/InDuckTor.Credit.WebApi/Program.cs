using System.Reflection;
using Hangfire;
using InDuckTor.Credit.Feature.Feature.Application;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Credit.WebApi.Configuration;
using InDuckTor.Credit.WebApi.Endpoints;
using InDuckTor.Shared.Configuration.Swagger;
using InDuckTor.Shared.Security.Http;
using InDuckTor.Shared.Security.Jwt;
using InDuckTor.Shared.Strategies;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddStrategiesFrom(Assembly.GetAssembly(typeof(ISubmitApplication))!)
    .ConfigureDomain(builder.Configuration);

services.AddProblemDetails().ConfigureJsonConverters();

services.AddLoanDbContext(builder.Configuration);

services.AddInDuckTorAuthentication(builder.Configuration.GetSection(nameof(JwtSettings)));
services.AddAuthorization();
services.AddInDuckTorSecurity();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(cfg =>
{
    cfg.ConfigureJwtAuth();
    cfg.ConfigureEnumMemberValues();
});

services.ConfigureHangfire(builder.Configuration);

services.ConfigureRefit(builder.Configuration);

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

app.UseInDuckTorSecurity();
app.UseHttpsRedirection();

app.UseHangfire();

app.Run();