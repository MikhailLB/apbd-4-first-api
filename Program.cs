using FirstCrudApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

MagazynDanych.Inicjalizuj();

var app = builder.Build();

app.MapControllers();
app.Run();
