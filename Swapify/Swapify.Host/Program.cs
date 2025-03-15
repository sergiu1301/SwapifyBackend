using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swapify.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swapify.Api;
using Swapify.Host.Swagger;
using Swapify.Host.Middlewares;
using Swapify.Notifications;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(configOption =>
{
    configOption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    configOption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    configOption.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtBearerOption =>
{
    jwtBearerOption.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = config["TokenSettings:Issuer"],
        ValidAudience = config["TokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["SwapifySettings:ApiSecret"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

    jwtBearerOption.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chathub") || path.StartsWithSegments("/connecthub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
}).AddJwtBearer(GoogleDefaults.AuthenticationScheme, options =>
{
    options.Authority = config["GoogleSettings:Issuer"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = config["GoogleSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = config["GoogleSettings:ClientId"],
        ValidateLifetime = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    config["GoogleSettings:ClientSecret"]!))
    };
});

builder.Services.AddDbContext<ApplicationDbContext>(
    optionsBuilder => optionsBuilder.UseSqlServer(config["ConnectionStrings:DefaultConnection"]!)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.InfrastructureConfigurations(config["ConnectionStrings:DefaultConnection"]!);

builder.Services.AddSwaggerGen();

builder.Services.AddLogging();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    }));

builder.Services.AddApiVersioning(o => {
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});
IDictionary<string, string> options = new Dictionary<string, string>
{
    { "Issuer", config["TokenSettings:Issuer"]! },
    { "Audience", config["TokenSettings:Audience"]! },
    { "ApiScope", config["SwapifySettings:ApiScope"]! },
    { "ApiSecret", config["SwapifySettings:ApiSecret"]! },
    { "GoogleClientId", config["GoogleSettings:ClientId"]! }
};
builder.Services.ApiConfigurations(options);
builder.Services.NotificationConfigurations();

builder.Services.AddSwaggerExampleConfigurations();

var app = builder.Build();

app.UseMiddleware<ForwardedPrefixHeaderMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "Swapify V1");
    });
    app.UseRouting();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

app.Run();
