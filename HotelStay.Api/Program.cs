using HotelStay.Api.Endpoints;
using HotelStay.Api.InfrastructureProviders;
using HotelStay.Api.Persistence;
using HotelStay.Api.Services;
using HotelStay.Api.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructureProviders();
builder.Services.AddApplicationServices();
builder.Services.AddValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHotelStayEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
