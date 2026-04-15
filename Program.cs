using FirstCrudApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opcje =>
{
    opcje.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Centrum Szkoleniowego",
        Version = "v1",
        Description = "Zarządzanie salami dydaktycznymi i ich rezerwacjami. " +
                      "Dane przechowywane w pamięci aplikacji (bez bazy danych)."
    });
});

MagazynDanych.Inicjalizuj();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(opcje =>
{
    opcje.SwaggerEndpoint("/swagger/v1/swagger.json", "API Centrum Szkoleniowego v1");
    opcje.RoutePrefix = string.Empty;
});

app.MapControllers();
app.Run();
