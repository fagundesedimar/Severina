using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Severina.API.Middlewares;
using Severina.Application.Behaviors;
using Severina.Application.Interfaces;
using Severina.Application.Services;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;
using Severina.Infrastructure.Data;
using Severina.Infrastructure.Repositories;
using Severina.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Severina API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddDbContext<SeverinaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IInviteCacheService, InMemoryInviteCacheService>();
builder.Services.AddScoped<IEmailService, MockEmailService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentCacheService, AppointmentCacheService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDashboardCacheService, DashboardCacheService>();
builder.Services.AddScoped<IImportService, CsvImportService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IExportJobRepository, ExportJobRepository>();
builder.Services.AddScoped<IFinancialCacheService, FinancialCacheService>();
builder.Services.AddScoped<IExportService, CsvExportService>();
builder.Services.AddHostedService<OverdueInvoiceDetectionService>();
builder.Services.AddSingleton<INotificationService, WebSocketNotificationService>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Severina.Application.DTOs.CompanyResponse).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Severina.Application.DTOs.CompanyResponse).Assembly);
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Severina.Application.Behaviors.ValidationBehavior<,>));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Administrador"));
    options.AddPolicy("AllAuthenticated", policy =>
        policy.RequireAuthenticatedUser());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("invite", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("appointment", limiterOptions =>
    {
        limiterOptions.PermitLimit = 30;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("dashboard", limiterOptions =>
    {
        limiterOptions.PermitLimit = 60;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("import", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("export", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseMiddleware<TenantMiddleware>();
app.UseWebSockets();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SeverinaDbContext>();
    context.Database.EnsureCreated();

    if (!context.Companies.Any())
    {
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var defaultPassword = "Test@123";
        var hash = passwordService.HashPassword(defaultPassword);

        var now = DateTime.UtcNow;

        // Empresa 1: CNPJ
        var company1 = new Company("Severina Demo Ltda", CnpjCpf.Create("11222333000181"), Email.Create("contato@severina-demo.com.br"), TipoPessoa.Juridica, Telefone.Create("11999998888"));
        context.Companies.Add(company1);
        context.SaveChanges();

        var admin1 = new User(company1.Id, "Admin Severina", Email.Create("admin@severina.com"), hash, PapelUsuario.Administrador);
        var user1 = new User(company1.Id, "Operador Severina", Email.Create("user@severina.com"), hash, PapelUsuario.Operacional);
        company1.AddUser(admin1);
        company1.AddUser(user1);
        context.Users.AddRange(admin1, user1);

        // Empresa 2: CPF
        var company2 = new Company("João Silva MEI", CnpjCpf.Create("52998224725"), Email.Create("joao@silva-mei.com.br"), TipoPessoa.Fisica, Telefone.Create("21988887777"));
        context.Companies.Add(company2);
        context.SaveChanges();

        var admin2 = new User(company2.Id, "João Silva", Email.Create("joao@teste.com"), hash, PapelUsuario.Administrador);
        company2.AddUser(admin2);
        context.Users.Add(admin2);

        context.SaveChanges();

        Console.WriteLine("Seed concluído. Usuários criados:");
        Console.WriteLine($"  Empresa 1 - admin@severina.com / Test@123 (Admin)");
        Console.WriteLine($"  Empresa 1 - user@severina.com / Test@123 (Operacional)");
        Console.WriteLine($"  Empresa 2 - joao@teste.com / Test@123 (Admin)");
    }
}

app.Run();

public partial class Program { }
