using System.Security.Claims;
using AdApi.Service;
using AdCore.Interface;
using AdCore.Response;
using AdCore.Services;
using AdCore.Settings;
using AdRepository;
using AdRepository.Authentication;
using AdService;
using AdService.Base;
using AdService.Interface;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region SeriLog

var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Logs");

if (!Directory.Exists(pathBuilt))
{
    Directory.CreateDirectory(pathBuilt);
}

builder.Host.UseSerilog((_, lc) => lc
    .WriteTo.File(pathBuilt + "\\log.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5242880,
        rollOnFileSizeLimit: true)
    // .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning));

#endregion

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMemoryCache();

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

#region CosmosDB
await builder.Services.InitializeCosmosClientAsync(builder.Configuration.GetSection("CosmosDb"));
#endregion

#region AzureADB2C Authentication and Authorization

builder.Services.AddSingleton(builder.Configuration.GetSection("AzureAdB2C").Get<B2CCredentials>());
builder.Services.AddSingleton<GraphClient>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.Configure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.NameClaimType = "name";
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = opt =>
            {
                string role = opt.Principal.FindFirstValue("extension_Role");
                if (role is not null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, role)
                    };

                    var appIdentity = new ClaimsIdentity(claims);
                    opt.Principal?.AddIdentity(appIdentity);
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new ApiResponse<string>("You are not Authorized"));
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new ApiResponse<string>("You are not authorized to access this resource"));
                return context.Response.WriteAsync(result);
            }
        };
    });

#endregion

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ad Management",
        Version = "v1",
        Description = "This Api will be responsible for overall data distribution and authorization.",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header,
            }, new List<string>()
        },
    });
});
#endregion

#region HealthCheck
builder.Services.AddHealthChecks();
builder.Services.AddHealthChecksUI().AddInMemoryStorage();
#endregion

#region Mail Service
builder.Services.AddSingleton(builder.Configuration.GetSection("EmailConfiguration").Get<MailSettings>());
builder.Services.AddSingleton<IMailService, MailService>();
#endregion

#region Services
builder.Services.AddScoped(typeof(IBaseService<,>), typeof(BaseService<,>));
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICurrentUserInfoService, CurrentUserInfoService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IAdService, AdService.AdService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(policy =>
    policy.WithOrigins("https://localhost:7239")
        .AllowAnyMethod()
        .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, "x-custom-header")
        .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI();

app.MapControllers();

app.Run();
