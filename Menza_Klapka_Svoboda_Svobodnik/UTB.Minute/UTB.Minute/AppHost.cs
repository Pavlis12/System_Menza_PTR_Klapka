var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder.AddPostgres("postgres");
var db = postgres.AddDatabase("minutedb");
var api = builder.AddProject<Projects.UTB_Minute_WebApi>("utb-minute-webapi")
                 .WithReference(db);

builder.AddProject<Projects.UTB_Minute_DbManager>("utb-minute-dbmanager")
       .WithReference(db);

builder.AddProject<Projects.UTB_Minute_AdminClient>("utb-minute-adminclient")
       .WithReference(api);

builder.AddProject<Projects.UTB_Minute_CanteenClient_Student>("utb-minute-canteenclient-student")
       .WithReference(api);

builder.AddProject<Projects.UTB_Minute_CanteenClient_Cook>("utb-minute-canteenclient-cook")
       .WithReference(api);

builder.Build().Run();

namespace UTB.Minute.Apphost
{
    public partial class Program { }
}