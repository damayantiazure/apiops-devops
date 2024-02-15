
using NeptureWebAPI;
using NeptureWebAPI.AzureDevOps;
using NeptureWebAPI.AzureDevOps.Security;
using NeptureWebAPI.AzureDevOps.Security.Schemes;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var aiConnectionString = AppConfig.GetAppInsightsConnStrFromEnv();
var enableAppInsights = !string.IsNullOrWhiteSpace(aiConnectionString);
// Add services to the container.
builder.Services.AddSingleton<AppConfig>();
if(enableAppInsights)
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = AppConfig.GetAppInsightsConnStrFromEnv();
    });
}
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                      builder =>
                      {
                          builder.WithOrigins("*",
                                              "*");
                      });
});

builder.Services.AddLogging(logging =>
{
    if(enableAppInsights)
    {
        logging.AddApplicationInsights(
            telemetryConfig =>
            {
                telemetryConfig.ConnectionString = AppConfig.GetAppInsightsConnStrFromEnv();
            },
            aiLoggingOptions =>
            {
                aiLoggingOptions.TrackExceptionsAsExceptionTelemetry = true;
            });
    }
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
    logging.AddConsole();
    logging.AddDebug();
});
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    AllowTrailingCommas = true,
    PropertyNameCaseInsensitive = true
});
builder.Services.AddSingleton<PersonalAccessTokenSupport>();
builder.Services.AddSingleton<ServicePrincipalTokenSupport>();
builder.Services.AddSingleton<ManagedIdentityTokenSupport>();
builder.Services.AddHttpClient(AppConfig.AZUREDEVOPSCLIENT, (services, client) => { client.BaseAddress = new Uri(AppConfig.AZDO_URI); });
builder.Services.AddHttpClient(AppConfig.AZUREDEVOPS_IDENTITY_CLIENT, (services, client) => { client.BaseAddress = new Uri(AppConfig.AZDO_IDENTITY_URI); });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Client>();
builder.Services.AddTransient<IdentitySupport>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// We will run this on a docker container, so we need to allow any host
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
