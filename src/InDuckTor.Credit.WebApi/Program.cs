using Hangfire;
using Hangfire.PostgreSql;
using InDuckTor.Credit.WebApi.Endpoints.LoanApplication;
using InDuckTor.Credit.WebApi.Endpoints.LoanProgram;
using InDuckTor.Shared.Security;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInDuckTorSecurity();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(configure =>
    {
        // configure.UseNpgsqlConnection();
    })
);

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

builder.Services.AddSwaggerGen(cfg =>
{
    cfg.AddSecurityDefinition("auth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "auth" }
            },
            []
        }
    });

    cfg.SwaggerDoc("v1", new OpenApiInfo { Title = "Blabber Service", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddLoanApplicationEndpoints()
    .AddLoanProgramEndpoints();

app.UseHttpsRedirection();

app.Run();