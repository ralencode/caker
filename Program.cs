using Caker.Data;
using Caker.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<CakerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

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

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
