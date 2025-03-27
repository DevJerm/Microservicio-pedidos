using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using pedidos_service.Application.Interfaces;
using pedidos_service.Application.Services;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.Services;
using pedidos_service.Infraestructure.Persistence.MongoDB;
using pedidos_service.Infraestructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080"); // Escuchar en todas las IP de la maquibna, para no tener problema con mi docker local

//utilizamos mongo porque solo va ser transaccional y no vamos a tener que generar consultar estructuradas, y por la facilidad que brinda mongo atlas 
var mongoDbSettings = builder.Configuration.GetSection("MongoDBSettings");
builder.Services.Configure<MongoDBSettings>(mongoDbSettings);
var settings = mongoDbSettings.Get<MongoDBSettings>();
Console.WriteLine($"ConnectionString: {settings?.ConnectionString ?? "null"}");
Console.WriteLine($"DatabaseName: {settings?.DatabaseName ?? "null"}");
// Configuración MongoDB
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));


// inyeccion de dependencias
builder.Services.AddSingleton<MongoDBContext>(); // Singleton para que solo haya una instancia de la conexion a la base de datos
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICreacionPedidoService, CreacionPedidoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//utilizo swagger para documentar la api 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservicio de Pedidos",
        Version = "v1",
        Description = "API para gestionar el ciclo de vida de pedidos de restaurante"
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();