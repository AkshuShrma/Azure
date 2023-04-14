using crudWithAzure.Data;
using crudWithAzure.Hub;
using crudWithAzure.models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITableStorageService<FileData>, TableStorageService<FileData>>();
builder.Services.AddScoped<IAuthenticateUser, Authenticate>();

//adding signalR
builder.Services.AddSignalR();

// adding cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "My", policy =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();

// Endpoint for signalR
app.MapHub<MessageHub>("/getDataBySignalR");
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Get All Data

app.MapGet("/getAllData/{id}", async (int id, ITableStorageService<FileData> tableStorageRepository) =>
{
    return Results.Ok(await tableStorageRepository.GetAllEntityAsync(id));
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
    entity.UserId = entity.UserId;
    await service.UpsertEntityAsync(entity);
    return Results.Ok("Updated");
})
    .WithName("UpdateData");

app.MapDelete("/Delete", async (string name, string id, string extension, ITableStorageService<FileData> tableStorageRepository) =>
{
    var getMessage = await tableStorageRepository.DeleteEntityAsync(name, id,extension);
    if (getMessage) return Results.Ok(new { Staus = 1, Message = "Deleted Successfully" });
    return Results.BadRequest(new { Staus = 0, Message = "Somehting went wrong" });

})
    .WithName("DeleteData");

// login user here 
app.MapGet("/login/{userName}/{password}", (string userName, string password, IAuthenticateUser iautenticateRepository) =>
{
    var data = iautenticateRepository.authenticateUser(userName, password);
    if (data != null) return Results.Ok(new { Status = 1, Message = "login successfully", data = new { userName = data.UserName, Id = data.RowKey } });
    return Results.BadRequest(new { Status = 0, Message = "login unsuccessfully" });
});


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