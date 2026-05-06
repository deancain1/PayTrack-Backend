using PayTrack.Api.Extensions;
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

var app = builder.Build();
app.UseApplicationPipeline();

app.Run();
