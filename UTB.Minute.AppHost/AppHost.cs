using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// 1. Definice SQL Serveru a konkrétní databáze
// Aspire se postará o to, aby ti v Dockeru nastartoval SQL kontejner.
var sql = builder.AddSqlServer("sqlserver");
var db = sql.AddDatabase("minutedb");

// 2. Registrace WebApi (to je to srdce systému)
// "webapi" je název služby, pod kterým ji uvidí ostatní
var api = builder.AddProject<Projects.UTB_Minute_WebApi>("webapi")
                 .WithReference(db); // Tímto říkáš: WebApi potřebuje přístup k databázi

// 3. Registrace klientských aplikací (zatím je tam dej jako placeholder)
builder.AddProject<Projects.UTB_Minute_AdminClient>("adminclient")
       .WithReference(api); // Admin mluví s API, ne s DB

builder.AddProject<Projects.UTB_Minute_CanteenClient>("canteenclient")
       .WithReference(api); // Student/Kuchařka mluví s API

builder.AddProject<Projects.UTB_Minute_DbManager>("utb-minute-dbmanager").WithReference(db);
Console.WriteLine("TEST");
builder.Build().Run();
