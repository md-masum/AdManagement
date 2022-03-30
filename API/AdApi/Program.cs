using System.Security.Claims;
using AdApi.Service;
using AdCore.Interface;
using AdCore.Settings;
using AdRepository;
using AdRepository.Authentication;
using AdService;
using AdService.Interface;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
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

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.File(pathBuilt + "\\log.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5242880,
        rollOnFileSizeLimit: true)
    // .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning));

#endregion

builder.Services.AddControllers();
builder.Services.AddScoped<IAdService, AdService.AdService>();
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
            }
        };
    });

#endregion

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region HealthCheck
builder.Services.AddHealthChecks();
builder.Services.AddHealthChecksUI().AddInMemoryStorage();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
