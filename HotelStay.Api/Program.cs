using HotelStay.Api.Endpoints;
using HotelStay.Api.InfrastructureProviders;
using HotelStay.Api.Persistence;
using HotelStay.Api.Services;
using HotelStay.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructureProviders();
builder.Services.AddApplicationServices();
builder.Services.AddValidation();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://hotelstay-booking-portal.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelStay API V1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
    .ExcludeFromDescription();
app.MapHotelStayEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await SeedData.EnsureSeededAsync(dbContext);
}

app.Run();
