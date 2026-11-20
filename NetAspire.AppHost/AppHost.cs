using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("SqlPassword", "Password12*", true);

#region product - mssql
var mssqlDb = builder.AddSqlServer("SqlServer").WithLifetime(ContainerLifetime.Persistent).WithHostPort(1233).WithPassword(password);
var productDb = mssqlDb.AddDatabase("ProductDb");

var rabbitmq = builder.AddRabbitMQ("rabbitMQ-2", null, null, 5672).WithLifetime(ContainerLifetime.Persistent)
    .WithImage("rabbitmq", "3-management") // yönetim için plugin (eklenti) imajı
    .WithHttpEndpoint(15672, targetPort:15672, name:"management") //web ui portu
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest");

var redis = builder.AddRedis("cache").WithLifetime(ContainerLifetime.Persistent).WithHostPort(6379);

builder.AddProject<NetAspire_Product_WebApi>("product").WithReference(mssqlDb).WithReference(redis).WithReference(rabbitmq).WaitFor(redis);
#endregion

//#region category - postgres
//var userName = builder.AddParameter("SqlPostgresUserName", "postgress", true);
//var postgresDb = builder.AddPostgres("Postgres", userName, password, 5332);
//var categoryDb = postgresDb.AddDatabase("categorydb");
//builder.AddProject<NetAspire_Category_WebApi>("category").WithReference(categoryDb).WaitFor(categoryDb);
//#endregion

//builder.AddProject<NetAspire_Basket_WebApi>("basket");

builder.Build().Run();
