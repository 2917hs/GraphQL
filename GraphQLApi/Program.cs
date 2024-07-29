using GraphQLApi.ConfigurationOptions;
using GraphQLApi.Database.Data;
using GraphQLApi.ServiceExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomCors();
builder.Host.UseCustomSerilog(builder.Configuration);
builder.Services.ConfigureMobileUiServices(builder.Configuration);

// Retrieve apiSettings after configuration
var apiSettings = builder.Configuration.GetSection("MobileUIServices").Get<OptionsConfiguration>();
builder.Services.Configure<TestKeysSettings>(builder.Configuration.GetSection("TestKeys"));

builder.Services.AddCustomGraphQl();
builder.Services.AddDbContext(builder.Configuration, builder.Environment);
builder.Services.AddCustomHttpClients(apiSettings);
builder.Services.Configure<TestKeysSettings>(builder.Configuration.GetSection("TestKeys"));
builder.Services.AddCustomServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<InMemoryDbContext>();
    //context.Database.Migrate();
    InMemoryDbContext.SeedData(context);
}

// Configure Middleware
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
//app.UseMiddleware<ETagMiddleware>();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapGraphQL();
app.UseGraphQLGraphiQL("/playground");

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
