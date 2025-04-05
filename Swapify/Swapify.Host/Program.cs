using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swapify.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swapify.Api;
using Swapify.Host.Swagger;
using Swapify.Host.Middlewares;
using Swapify.Notifications;
using Swapify.Host.Settings;
using Microsoft.Extensions.Options;
using Swapify.Host.Swaggers;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swapify.API.Controllers;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

config.AddSettingsConfigurations();
builder.Services.AddSettings();

var sp = builder.Services.BuildServiceProvider();
var tokenSettings = sp.GetRequiredService<IOptions<TokenSettings>>().Value;
var swapifySettings = sp.GetRequiredService<IOptions<SwapifySettings>>().Value;
var connectionStrings = sp.GetRequiredService<IOptions<ConnectionStrings>>().Value;

builder.Services.AddControllers();
builder.Services.AddSignalR();
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
        ValidIssuer = tokenSettings.Issuer,
        ValidAudience = tokenSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(swapifySettings.ApiSecret)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        NameClaimType = JwtRegisteredClaimNames.Sub,
    };

    jwtBearerOption.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub/status"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthentication("MyCookieScheme")
    .AddCookie("MyCookieScheme", options =>
    {
        options.Cookie.Name = "Swapify";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddDbContext<ApplicationDbContext>(
    optionsBuilder => optionsBuilder.UseSqlServer(connectionStrings.DefaultConnection)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.InfrastructureConfigurations(connectionStrings.DefaultConnection);
builder.Services.ApiConfigurations(
    tokenSettings.Issuer, 
    tokenSettings.Audience, 
    swapifySettings.ApiSecret, 
    swapifySettings.ClientSecret);
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddHttpContextAccessor();
/*builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    }));
*/
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
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
builder.Services.NotificationConfigurations();
builder.Services.AddSwaggerExampleConfigurations();

var app = builder.Build();
app.UseStaticFiles();
app.UseMiddleware<ForwardedPrefixHeaderMiddleware>();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "Swapify V1");
        c.OAuthClientId("swagger.pkce");
        c.OAuthClientSecret("");
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<StatusHub>("/hub/status");
app.UseMiddleware<ExceptionMiddleware>();

app.Run();
