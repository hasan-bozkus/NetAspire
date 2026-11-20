using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Caching.Distributed;
using NetAspire.Product.WebApi.Context;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlServerDbContext<ApplicationDbContext>("ProductDb");

builder.AddRedisDistributedCache("cache");

builder.AddRabbitMQClient("messaging");

builder.AddServiceDefaults();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

Product.SeedDataProduct();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/products", (IDistributedCache cache) =>
{
    List<Product>? res;
    var stringRes= cache.GetString("products");
    if(stringRes is null)
    {
        res = Product.Products;
        cache.SetString("products", JsonSerializer.Serialize(res));
    }
    else
    {
        res = JsonSerializer.Deserialize<List<Product>>(stringRes);
    }
    return Results.Ok(res);
});
app.MapGet("/update-database", (ApplicationDbContext dbContext) =>
{
    dbContext.Database.Migrate();
    return Results.Ok(new { message = "Veri tabanı başarıyla güncellendi." });
});

//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    dbContext.Database.Migrate();
//}

app.MapDefaultEndpoints();

app.Run();
