
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TerraPulse.Api.Data;
using TerraPulse.Api.Exceptions;
using TerraPulse.Api.Mappings;
using TerraPulse.Api.Repositories;
using TerraPulse.Api.Services;
using TerraPulse.Api.ViewModels.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TerraPulseDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<TerraPulseMappingProfile>();
});

builder.Services.AddScoped<IAreaMonitoradaRepository, AreaMonitoradaRepository>();
builder.Services.AddScoped<IAreaMonitoradaService, AreaMonitoradaService>();

builder.Services.AddScoped<IIndicadorAmbientalRepository, IndicadorAmbientalRepository>();
builder.Services.AddScoped<IIndicadorAmbientalService, IndicadorAmbientalService>();

builder.Services.AddScoped<IAlertaAmbientalRepository, AlertaAmbientalRepository>();
builder.Services.AddScoped<IAlertaAmbientalService, AlertaAmbientalService>();

builder.Services.AddScoped<IRecomendacaoSustentavelRepository, RecomendacaoSustentavelRepository>();
builder.Services.AddScoped<IRecomendacaoSustentavelService, RecomendacaoSustentavelService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("A chave JWT nao foi configurada no appsettings.json.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        var response = new ValidationErrorResponseViewModel
        {
            Message = "Existem campos invalidos na requisicao.",
            StatusCode = StatusCodes.Status400BadRequest,
            Timestamp = DateTime.UtcNow,
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TerraPulse.Api",
        Version = "v1",
        Description = "API RESTful para monitoramento ambiental, indicadores ESG, alertas e recomendacoes sustentaveis."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Cole somente o accessToken gerado no login. O Swagger enviara como Bearer Token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Migrations automaticas: sempre no ambiente "Docker" e, nos demais ambientes
// (CI, Staging, Production), quando a variavel Database__ApplyMigrations=true.
var aplicarMigrations = app.Environment.IsEnvironment("Docker")
    || app.Configuration.GetValue<bool>("Database:ApplyMigrations");

if (aplicarMigrations)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TerraPulseDbContext>();

    var maxTentativas = 12;

    for (var tentativa = 1; tentativa <= maxTentativas; tentativa++)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch
        {
            if (tentativa == maxTentativas)
            {
                throw;
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }
}

// Swagger habilitado em Development/Docker ou quando Swagger__Enabled=true
// (usado em staging e producao para evidenciar o funcionamento da API).
var swaggerHabilitado = app.Environment.IsDevelopment()
    || app.Environment.IsEnvironment("Docker")
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerHabilitado)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

// Health check usado pelo pipeline (smoke test pos-deploy) e para evidencias.
// Retorna o ambiente e a versao (SHA do commit) que esta rodando.
app.MapGet("/health", async (TerraPulseDbContext db, IConfiguration config, IWebHostEnvironment env) =>
{
    var bancoOk = false;
    try
    {
        bancoOk = await db.Database.CanConnectAsync();
    }
    catch
    {
        bancoOk = false;
    }

    var payload = new
    {
        status = bancoOk ? "Healthy" : "Unhealthy",
        environment = env.EnvironmentName,
        version = config["APP_VERSION"] ?? "local",
        database = bancoOk ? "Connected" : "Unavailable",
        timestamp = DateTime.UtcNow
    };

    return bancoOk
        ? Results.Ok(payload)
        : Results.Json(payload, statusCode: StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
