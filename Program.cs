using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Core;
using Microsoft.Data.SqlClient;
using TodoApi.Data;
using TodoApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 🔐 AzureAd-kredentiaalien asetus
builder.Services.Configure<AzureAdOptions>(
    builder.Configuration.GetSection("AzureAd"));

// 🌐 SQL Server -yhteyden muodostus AccessTokenilla
builder.Services.AddDbContext<TodoContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var azureAd = sp.GetRequiredService<IOptions<AzureAdOptions>>().Value;
    var connectionString = config.GetConnectionString("SqlServer");

    var credential = new ClientSecretCredential(
        azureAd.TenantId,
        azureAd.ClientId,
        azureAd.ClientSecret);

    var token = credential.GetToken(
        new TokenRequestContext(new[] { "https://database.windows.net/.default" }));

    var conn = new SqlConnection(connectionString)
    {
        AccessToken = token.Token
    };

    options.UseSqlServer(conn);
});

// Configure Azure AD authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add authorization
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS if needed for frontend applications
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3001") // Add your frontend URLs
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// 🛠️ Automaattinen tietokantamigraatio
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

// Use CORS
app.UseCors("AllowSpecificOrigin");

// Comment out HTTPS redirect in development to avoid the warning
// app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add a simple health check endpoint that doesn't require authentication
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName,
    Message = "Todo API is running successfully!"
}).WithOpenApi();

// Add info endpoint to check configuration (development only)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/info", (IConfiguration config) => new
    {
        HasAzureAdConfig = !string.IsNullOrEmpty(config["AzureAd:TenantId"]),
        HasConnectionString = !string.IsNullOrEmpty(config.GetConnectionString("DefaultConnection")),
        AzureAdInstance = config["AzureAd:Instance"],
        AzureAdDomain = config["AzureAd:Domain"],
        Message = "Check your appsettings.Development.json configuration"
    }).WithOpenApi();
}

app.Run();
