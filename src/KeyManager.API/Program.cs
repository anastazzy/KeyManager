using KeyManager.API.Extensions;
using KeyManager.API.Utils;
using KeyManager.Application.Contracts;
using KeyManager.Application.Services;
using KeyManager.DataAccess;
using KeyManager.Infrastructure.Contracts;
using KeyManager.Infrastructure.Dtos;
using KeyManager.Infrastructure.Services;
using KeyManager.MailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<KeyManagerDbContext>(x =>
    x.UseNpgsql(builder.Configuration.GetConnectionString("Postgre")));

builder.Services.AddTransient<IJwtProvider, JwtProvider>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IApiKeyService, ApiKeyService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<IServiceApiKeyManager, ServiceApiKeyManager>();

builder.Services.AddScoped<ServiceAuthorizationActionFilter>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpOptions"));

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
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
    options.AddOperationFilterInstance(new SwaggerHeaderOptions());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();