using System.Reflection;
using InDuckTor.Credit.Feature.Feature.Application;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Credit.WebApi.Configuration;
using InDuckTor.Credit.WebApi.Endpoints;
using InDuckTor.Shared.Configuration.Swagger;
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
// services.AddInDuckTorSecurity();

services.AddEndpointsApiExplorer();
services.AddCreditSwaggerGen();

services.ConfigureHangfire(builder.Configuration);

services.ConfigureRefit(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddLoanApplicationEndpoints()
    .AddLoanProgramEndpoints()
    .AddLoanEndpoints()
    .AddTestEndpoints();

// app.UseInDuckTorSecurity();
app.UseHttpsRedirection();

app.UseHangfire();

app.Run();