using PayTrack.Api.Extensions;
using PayTrack.Infrastructure.Hubs;



//using PayTrack.Infrastructure.Seeding;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCustomCors(builder.Configuration)
                .AddApplicationServices()
                .AddMediatRServices()
                .AddDatabaseAndIdentity(builder.Configuration)
                 .AddJwtAuthentication(builder.Configuration)
                .AddSwaggerWithAuth();
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
//}
app.UseApplicationPipeline();
app.MapHub<NotificationHub>("/notificationHub");
app.Run();
