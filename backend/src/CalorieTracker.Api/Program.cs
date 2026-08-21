using CalorieTracker.Infrastructure.Data;
using CalorieTracker.Infrastructure.ExternalApis;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Подключение базы данных SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=calorietracker.db"));

// Регистрация HttpClient
builder.Services.AddHttpClient<OpenFoodFactsClient>()
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        AutomaticDecompression = System.Net.DecompressionMethods.GZip 
                               | System.Net.DecompressionMethods.Deflate
    });      

// Добавление контроллеров
builder.Services.AddControllers();

// Регистрация сервисов для Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Настройка конвейера обработки HTTP-запросов (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Генерация интерфейса Swagger по адресу /swagger
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();