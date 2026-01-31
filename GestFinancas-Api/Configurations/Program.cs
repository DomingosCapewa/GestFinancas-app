using GestFinancas_Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GestFinancas_Api.Models;
using GestFinancas_Api.Helper;
using GestFinancas_Api.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using GestFinancas_Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
    ?? builder.Configuration["Jwt:SecretKey"];

// Validar configurações críticas em produção
if (builder.Environment.IsProduction())
{
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Connection string não configurada. Configure a variável de ambiente DB_CONNECTION_STRING.");
    
    if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 64)
        throw new InvalidOperationException("JWT Secret Key deve ter no mínimo 64 caracteres. Configure a variável de ambiente JWT_SECRET_KEY.");
}

if (string.IsNullOrEmpty(jwtSecret))
{
    Console.WriteLine(" ATENÇÃO: Usando chave JWT padrão (APENAS DESENVOLVIMENTO)");
    jwtSecret = "veryverycomplexkey1234567890-development-only-change-in-production";
}

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddScoped<EnviarEmail>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthenticate, Authenticate>();
builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.RequireHttpsMetadata = builder.Environment.IsProduction();
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"],
    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
    ClockSkew = TimeSpan.FromMinutes(5)
  };
});

var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') 
    ?? builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    allowedOrigins = new[] { "http://localhost:3000", "http://localhost:4200", "http://localhost:5282", "https://localhost:5282" };
}

builder.Services.AddCors(options =>
{
  options.AddPolicy("AppCorsPolicy", policy =>
  {
    policy.WithOrigins(allowedOrigins)
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
    
    if (builder.Environment.IsProduction())
    {
        Console.WriteLine($"✓ CORS configurado para produção: {string.Join(", ", allowedOrigins)}");
    }
  });
});


builder.Services.AddApiVersioning(options =>
{
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.ReportApiVersions = true;
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
  options.EnableAnnotations();
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "GestFinancas API",
    Version = "v1",
    Description = "API para gerenciamento de usuários e finanças",
    Contact = new OpenApiContact
    {
      Name = "Suporte",
      Email = "equipe.gest.financas@gmail.com"
    }
  });
  
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = @"JWT Authorization header usando Bearer.
                    Entre com 'Bearer ' [espaço] então coloque seu token.
                    Exemplo: 'Bearer 12345abcdef'",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });
  
  options.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
      },
      Array.Empty<string>()
    }
  });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "GestFinancas API v1");
  c.RoutePrefix = string.Empty;
  
  if (app.Environment.IsProduction())
  {
    c.DocumentTitle = "GestFinancas API - Production";
  }
});

app.UseCors("AppCorsPolicy");

if (app.Environment.IsProduction())
{
  app.UseHttpsRedirection();
  app.UseHsts();
}

app.Use(async (context, next) =>
{
  context.Response.Headers["X-Content-Type-Options"] = "nosniff";
  context.Response.Headers["X-Frame-Options"] = "DENY";
  context.Response.Headers["Referrer-Policy"] = "no-referrer";
  context.Response.Headers["X-XSS-Protection"] = "0";
  context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

  if (app.Environment.IsProduction())
  {
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; frame-ancestors 'none'";
  }

  await next();
});

// Rate limiting (antes de autenticação)
app.UseIpRateLimiting();


app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<AuditMiddleware>();
app.UseAuthorization();


app.MapControllers();

app.Run();
