using Npgsql;
using FastEndpoints;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Harc.Api.Modules.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Harc.Api.Modules.Identity.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson(); // İşte hatayı çözen o sihirli satır!
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(dataSource));


var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = googleClientId,
            ValidateLifetime = true,
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

// 3. Şemadaki 4. Adım: Claims Transformation Servisimizi Kaydediyoruz
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

// 3. FASTENDPOINTS SERVISI (Carter ve MediatR yerine tek güç)
builder.Services.AddFastEndpoints();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Harc API Portal")
               .WithTheme(ScalarTheme.DeepSpace);
    });
}

// 4. KORUMA ARA KATMANLARI (Middleware) - Sıralama çok kritiktir!
app.UseAuthentication(); // Kimsin? (Google Token Kontrolü)
app.UseAuthorization();  // Yetkin var mı? (Claims/Role Kontrolü)

app.UseFastEndpoints(); // FastEndpoints rotalarını ekliyoruz

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    dbContext.Database.Migrate();
}

app.Run();