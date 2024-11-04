using Microsoft.Extensions.Options;
using Telegram.Bot;
using NotAPidorBot;
using NotAPidorBot.Services;
using NotAPidorBot.Configurations;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register Bot configurations
        services.Configure<BotConfiguration>(context.Configuration.GetSection("BotConfiguration"));
        services.Configure<CharacterConfiguration>(context.Configuration.GetSection("CharacterConfiguration"));
        services.Configure<ContextConfiguration>(context.Configuration.GetSection("ContextConfiguration"));

        // Register named HttpClient to benefits from IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read: 
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
                    CharacterConfiguration? characterConfiguration = sp.GetService<IOptions<CharacterConfiguration>>()?.Value;
                    ContextConfiguration? contextConfiguration = sp.GetService<IOptions<ContextConfiguration>>()?.Value;
                    ArgumentNullException.ThrowIfNull(botConfiguration);
                    ArgumentNullException.ThrowIfNull(characterConfiguration);
                    ArgumentNullException.ThrowIfNull(contextConfiguration);
                    Settings.Init(botConfiguration, characterConfiguration, contextConfiguration);
                    TelegramBotClientOptions options = new(botConfiguration.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();
