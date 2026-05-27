using System.Data;
using FGVTest.Api.Mappings;
using FGVTest.Api.Middlewares;
using FGVTest.Business.Mappings;
using FGVTest.Business.Interfaces.Repositories;
using FGVTest.Business.Interfaces.Services;
using FGVTest.Business.Services;
using FGVTest.Data.Repositories;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(BusinessMappingProfile).Assembly);
    cfg.AddMaps(typeof(ApiMappingProfile).Assembly);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "FGV Sales API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FGV Sales API v1");
});

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
