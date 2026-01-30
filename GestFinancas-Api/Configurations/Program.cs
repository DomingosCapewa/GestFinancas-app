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

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "veryverycomplexkey1234567890";

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


builder.Services.AddScoped<EnviarEmail>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthenticate, Authenticate>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "veryverycomplexkey1234567890";
  
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
  };
});

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowLocalDev", policy =>
  {
    policy.WithOrigins(
            "http://localhost:3000", 
            "http://localhost:4200",
            "http://localhost:5282",
            "https://localhost:5282")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
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


  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GestFinancas API v1");
    c.RoutePrefix = string.Empty;
  });
}
app.UseCors("AllowLocalDev");


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
