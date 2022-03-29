using System.Security.Claims;
using AdCore.Interface.Repository;
using AdCore.Interface.Service;
using AdRepository;
using AdRepository.Settings;
using AdService;
using AzureADB2CApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddSingleton<ITestRepository, TestRepository>();

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.MapControllers();

app.Run();
