using Amazon.Runtime;
using Amazon.S3;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SFM_BE.Contexts;
using SFM_BE.Mappings;
using SFM_BE.Middlewares;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using SFM_BE.Services;
using SFM_BE.Services.Auth;
using SFM_BE.Services.Provider;
using SFM_BE.Services.User;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

LoadEnvironmentVariables();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var jwtIssuer = GetRequiredConfiguration(builder.Configuration, "JWT_ISSUER");
var jwtAudience = GetRequiredConfiguration(builder.Configuration, "JWT_AUDIENCE");
var jwtKey = GetRequiredConfiguration(builder.Configuration, "JWT_KEY");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
        {
            string[] adminRoles = ["Admin", "ADMIN", "admin"];

            return adminRoles.Any(role => context.User.IsInRole(role))
                || context.User.Claims.Any(claim =>
                    (claim.Type == ClaimTypes.Role || claim.Type == "role")
                    && adminRoles.Any(role =>
                        string.Equals(claim.Value, role, StringComparison.OrdinalIgnoreCase)));
        }));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<S3PresignedUrlService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddHttpClient<IExternalAuthProvider, GoogleAuthProvider>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IAmazonS3>(_ =>
    new AmazonS3Client(
        new BasicAWSCredentials(
            GetRequiredConfiguration(builder.Configuration, "ACCESS_KEY_ID"),
            GetRequiredConfiguration(builder.Configuration, "SECRET_ACCESS_KEY")),
        Amazon.RegionEndpoint.GetBySystemName(
            GetRequiredConfiguration(builder.Configuration, "AWS_REGION"))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", document),
                []
            }
        });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static string GetRequiredConfiguration(IConfiguration configuration, string key)
{
    return configuration[key]
        ?? throw new InvalidOperationException($"{key} is missing.");
}

static void LoadEnvironmentVariables()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory != null)
    {
        var envPath = Path.Combine(directory.FullName, ".env");

        if (File.Exists(envPath))
        {
            Env.Load(envPath);
            return;
        }

        directory = directory.Parent;
    }
}
