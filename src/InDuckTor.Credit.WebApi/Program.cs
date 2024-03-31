using System.Reflection;
using InDuckTor.Credit.Feature.Feature.Application;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Credit.WebApi.Configuration;
using InDuckTor.Credit.WebApi.Configuration.Exceptions;
using InDuckTor.Credit.WebApi.Endpoints;
using InDuckTor.Credit.WebApi.Endpoints.Application;
using InDuckTor.Credit.WebApi.Endpoints.Application.V1;
using InDuckTor.Credit.WebApi.Endpoints.CreditScore;
using InDuckTor.Credit.WebApi.Endpoints.Loan;
using InDuckTor.Credit.WebApi.Endpoints.Loan.V1;
using InDuckTor.Credit.WebApi.Endpoints.Program;
using InDuckTor.Credit.WebApi.Endpoints.Program.V1;
using InDuckTor.Shared.Security.Http;
using InDuckTor.Shared.Security.Jwt;
using InDuckTor.Shared.Strategies;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddStrategiesFrom(Assembly.GetAssembly(typeof(ISubmitApplication))!)
    .ConfigureDomain(builder.Configuration)
    .ConfigureDomainEvents();

services.AddProblemDetails().ConfigureJsonConverters();
services.AddExceptionHandler<GlobalExceptionHandler>();

services.AddLoanDbContext(builder.Configuration);

services.AddAuthorization();
services.AddInDuckTorAuthentication(builder.Configuration.GetSection(nameof(JwtSettings)));
services.AddInDuckTorSecurity();

services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
services.AddEndpointsApiExplorer();

services.AddCreditSwaggerGen();

services.ConfigureHangfire(builder.Configuration);

services.ConfigureRefit(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddLoanApplicationEndpoints()
    .AddLoanProgramEndpoints()
    .AddLoanEndpoints()
    .AddCreditScoreEndpoints()
    .AddTestEndpoints();

app.UseExceptionHandler();

app.UseCors();

// Не менять порядок!!!
app.UseAuthentication();
app.UseAuthorization();
app.UseInDuckTorSecurity();

// app.UseHttpsRedirection();

app.UseHangfire();

app.Run();