using Azure.Data.Tables;
using crudWithAzure.Data;

using crudWithAzure.Hubs;
using crudWithAzure.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton(c => new TableServiceClient(Environment.GetEnvironmentVariable("table")));

// Add services
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
app.UseCors("My");
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

// get single user here 
app.MapGet("/getentityasync/{fileName}/{id}", async (string fileName, string id, ITableStorageService<FileData> service) =>
{
    return Results.Ok(await service.GetEntityAsync(fileName, id));
});

// add user here 
app.MapPost("/upsertentityasync", async (FileData entity, ITableStorageService<FileData> service) =>
{
    entity.PartitionKey = entity.FileName;
    string Id = Guid.NewGuid().ToString();
    entity.Id = Id;
    entity.RowKey = Id;
    var createdEntity = await service.CreateRecord(entity);
    return createdEntity;
});

// update user here 
app.MapPut("/updateentityasync", async (FileData entity, ITableStorageService<FileData> service) =>
{
    entity.PartitionKey = entity.PartitionKey;
    entity.RowKey = entity.Id;
    entity.UserId = entity.UserId;
    await service.UpsertEntityAsync(entity);
    return Results.Ok("Updated");
});

// Delete user here 
app.MapDelete("/Delete/{name}/{id}/{extension}/{partitionKey}", async (string name, string id, string extension,string partitionKey, ITableStorageService<FileData> tableStorageRepository) =>
{
    var getMessage = await tableStorageRepository.DeleteEntityAsync(name, id, extension, partitionKey);
    if (getMessage) return Results.Ok(new { Staus = 1, Message = "Deleted Successfully" });
    return Results.BadRequest(new { Staus = 0, Message = "Somehting went wrong" });

});

// login user here 
app.MapGet("/login/{userName}/{password}", (string userName, string password, IAuthenticateUser iautenticateRepository) =>
{
    var data = iautenticateRepository.authenticateUser(userName, password);
    if (data != null) return Results.Ok(new { Status = 1, Message = "login successfully", data = new { userName = data.UserName, Id = data.RowKey } });
    return Results.BadRequest(new { Status = 0, Message = "login unsuccessfully" });
});

app.Run();

public partial class Program { }