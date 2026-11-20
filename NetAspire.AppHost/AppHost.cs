using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDotnetProject("product", @"..\NetAspire.Product.WebApi\NetAspire.Product.WebApi.csproj");
builder.AddDotnetProject("category", @"..\NetAspire.Category.WebApi\NetAspire.Category.WebApi.csproj");
builder.AddProject("basket", @"..\NetAspire.Basket.WebApi\NetAspire.Basket.WebApi.csproj");

builder.Build().Run();
