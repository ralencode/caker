using Caker.Data;
using Caker.Repositories;
using Caker.Services.ImageService;
using Caker.Services.PasswordService;
using Microsoft.EntityFrameworkCore;

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

// Add password service
builder.Services.AddScoped<IPasswordService, BCryptPasswordService>();

// Add image service
builder.Services.AddScoped<IImageService, FtpImageService>();

// Add Repositories
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<OrderRepository>();
builder.Services.AddTransient<ConfectionerRepository>();
builder.Services.AddTransient<CustomerRepository>();
builder.Services.AddTransient<CakeRepository>();
builder.Services.AddTransient<FeedbackRepository>();
builder.Services.AddTransient<MessageRepository>();

// Add Controllers
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

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

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
