using System.Reflection;
using Hangfire;
using Hangfire.PostgreSql;
using InDuckTor.Credit.Feature.Feature.Application;
using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Credit.WebApi.Configuration;
using InDuckTor.Credit.WebApi.Endpoints;
using InDuckTor.Shared.Configuration;
using InDuckTor.Shared.Security;
using InDuckTor.Shared.Strategies;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddStrategiesFrom(Assembly.GetAssembly(typeof(ISubmitApplication))!)
    .ConfigureDomain(builder.Configuration);

services.AddProblemDetails().ConfigureJsonConverters();

services.AddLoanDbContext(builder.Configuration);
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

app.UseHangfireDashboard();

app.AddLoanApplicationEndpoints()
    .AddLoanProgramEndpoints()
    .AddLoanEndpoints()
    .MapHangfireDashboard();

app.UseHttpsRedirection();

app.RunBackgroundJobs();

app.Run();