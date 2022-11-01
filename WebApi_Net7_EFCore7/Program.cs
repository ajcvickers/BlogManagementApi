using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UnicornSupplies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContextPool<UnicornSuppliesContext>(
    b =>
    {
        b.UseSqlServer("name=UnicornSupplies");
    });

builder.Services.AddControllers()
    .AddNewtonsoftJson(s =>
    {
        s.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
