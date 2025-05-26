using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Caker.Data;
using Caker.Repositories;
using Caker.Services;
using Caker.Services.CurrentUserService;
using Caker.Services.ImageService;
using Caker.Services.PasswordService;
using Caker.Services.TokenService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure lowercase URLs
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// Add DbContext
builder.Services.AddDbContext<CakerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null
            );
            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    )
);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSwagger",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:8085", "https://localhost:8085")
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );

    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("https://your-frontend-domain.com")
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

// Add password service
builder.Services.AddScoped<IPasswordService, BCryptPasswordService>();

// Add image service
builder.Services.AddScoped<IImageService, LocalFileImageService>();

// Add token service
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHostedService<ExpiredTokenCleaner>();

// Add authentication and authorization
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Configure JWT Authentication with Bearer header
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            ),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "ADMIN"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireClaim(ClaimTypes.Role, "CUSTOMER"));
    options.AddPolicy(
        "ConfectionerOnly",
        policy => policy.RequireClaim(ClaimTypes.Role, "CONFECTIONER")
    );
});

// Add Repositories
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<OrderRepository>();
builder.Services.AddTransient<ConfectionerRepository>();
builder.Services.AddTransient<CustomerRepository>();
builder.Services.AddTransient<CakeRepository>();
builder.Services.AddTransient<RefreshTokenRepository>();

// Add Controllers
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.SnakeCaseLower)
        );
        options.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.AllowNamedFloatingPointLiterals;
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Caker API",
            Version = "v1",
            Description = "API for cake ordering and management system",
            Contact = new OpenApiContact { Name = "Support", Email = "support@caker.ralen.top" },
        }
    );

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add Bearer token support to Swagger
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

var app = builder.Build();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CakerDbContext>();
    await db.Database.MigrateAsync();
}

// Middleware pipeline
app.UseExceptionHandler("/error");
app.Map(
    "/error",
    (HttpContext context) => Results.Problem(statusCode: context.Response.StatusCode)
);

app.UseHttpsRedirection();
app.UseRouting();

// Apply CORS, Authentication, and Authorization
app.UseCors("AllowSwagger");
app.UseCors("AllowFrontend");
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Caker API v1"));

app.MapControllers();

app.Run();
