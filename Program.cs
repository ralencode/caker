using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Caker.Data;
using Caker.Repositories;
using Caker.Services.CurrentUserService;
using Caker.Services.ImageService;
using Caker.Services.PasswordService;
using Caker.Services.TokenService;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
            policy.WithOrigins("http://localhost:8085").AllowAnyMethod().AllowAnyHeader();
            policy.WithOrigins("https://localhost:8085").AllowAnyMethod().AllowAnyHeader();
            policy.WithOrigins("http://swagger:8080").AllowAnyMethod().AllowAnyHeader();
            policy.WithOrigins("https://swagger:8080").AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Add password service
builder.Services.AddScoped<IPasswordService, BCryptPasswordService>();

// Add image service
builder.Services.AddScoped<IImageService, LocalFileImageService>();

// Add token service
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHostedService<Caker.Services.ExpiredTokenCleaner>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "ADMIN"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireClaim(ClaimTypes.Role, "CUSTOMER"));
    options.AddPolicy(
        "ConfectionerOnly",
        policy => policy.RequireClaim(ClaimTypes.Role, "CONFECTIONER")
    );
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

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
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
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
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CakerDbContext>();
    db.Database.Migrate();
}

// Add error handling
app.UseExceptionHandler("/error");
app.Map(
    "/error",
    (HttpContext context) => Results.Problem(statusCode: context.Response.StatusCode)
);

// Use swagger
app.UseCors("AllowSwagger");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Caker API v1");
});

// Exception handler
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                new { error = "An unexpected error occurred", requestId = context.TraceIdentifier }
            )
        );
    });
});

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
