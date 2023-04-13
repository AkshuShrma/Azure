using crudWithAzure.Data;
using crudWithAzure.models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITableStorageService<FileData>, TableStorageService<FileData>>();

// adding cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "My", policy =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/getAllData", async (ITableStorageService<FileData> service) =>
{
    return Results.Ok(await service.GetAllEntityAsync());
});

app.MapGet("/getentityasync", async (string fileName, string id, ITableStorageService<FileData> service) =>
{
    return Results.Ok(await service.GetEntityAsync(fileName, id));
})
    .WithName("GetData");

app.MapPost("/upsertentityasync", async (FileData entity, ITableStorageService<FileData> service) =>
{
    entity.PartitionKey = entity.FileName;
    string Id = Guid.NewGuid().ToString();
    entity.Id = Id;
    entity.RowKey = Id;
    var createdEntity = await service.UpsertEntityAsync(entity);
    return createdEntity;
})
    .WithName("PostData");

app.MapPut("/updateentityasync", async (FileData entity, ITableStorageService<FileData> service) =>
{
    entity.PartitionKey = entity.FileName;
    entity.RowKey = entity.Id;
    await service.UpsertEntityAsync(entity);
    return Results.NoContent();
})
    .WithName("UpdateData");

app.MapDelete("/deleteentityasync", async (string fileName, string id, ITableStorageService<FileData> service) =>
{
    await service.DeleteEntityAsync(fileName, id);
    return Results.NoContent();
})
    .WithName("DeleteData");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");



app.UseCors("My");


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}