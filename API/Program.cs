using DocumentManagement.Application;
using DocumentManagement.Application.DTOs;
using DocumentManagement.Application.UseCases;
using DocumentManagement.Domain.Entities;
using DocumentManagement.Domain.Enums;
using DocumentManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok("Document management API ready"));

app.MapPost("/folders", async (CreateFolderRequest request, FolderUseCaseService service) =>
{
    var admin = new UserAccount(Guid.NewGuid(), "Administrador", "admin@example.com", UserRole.Administrator);
    var response = await service.CreateFolderAsync(request, admin);
    return Results.Ok(response);
});

app.Run();
