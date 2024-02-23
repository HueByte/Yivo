using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yivo.Core.Options;
using Yivo.Core.Services.CommandHandler;
using Yivo.Core.Services.EventHandler;
using Yivo.Core.Services.MessageCache;
using Yivo.Core.Services.ServerInteraction;
using Yivo.Infrastructure;

namespace Yivo.Bot.Configurators

{
    public class InjectionConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public InjectionConfiguration(IConfiguration configuration, IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;
        }

        public InjectionConfiguration AddYivoCore()
        {
            DiscordShardedClient client = new(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.Guilds
                    | GatewayIntents.GuildBans
                    | GatewayIntents.GuildEmojis
                    | GatewayIntents.GuildIntegrations
                    | GatewayIntents.GuildWebhooks
                    | GatewayIntents.GuildInvites
                    | GatewayIntents.GuildVoiceStates
                    | GatewayIntents.GuildMessageReactions
                    | GatewayIntents.DirectMessageReactions
                    | GatewayIntents.GuildScheduledEvents
                    | GatewayIntents.GuildMembers
            });

            InteractionServiceConfig interactionServiceConfig = new()
            {
                AutoServiceScopes = false,
                DefaultRunMode = RunMode.Async,
            };

            InteractionService interactionService = new(client, interactionServiceConfig);

            _services.AddHostedService<YivoHost>()
                     .AddSingleton(client)
                     .AddSingleton(interactionService)
                     .AddSingleton<ICommandHandlerService, CommandHandlerService>()
                     .AddSingleton<IEventHandlerService, EventHandlerService>()
                     .AddMemoryCache();

            return this;
        }

        public InjectionConfiguration AddServices()
        {
            _services.AddScoped<IMessageCacheService, MessageCacheService>()
                     .AddScoped<IServerInteractionService, ServerInteractionService>();

            return this;
        }

        public InjectionConfiguration AddOptions()
        {
            _services.AddOptions<BotOptions>().Bind(_configuration.GetSection(BotOptions.BOT)).ValidateDataAnnotations();

            return this;
        }

        public InjectionConfiguration AddDatabaseServices()
        {
            var databaseConnString = _configuration.GetConnectionString("YivoContext");
            _services.AddYivoSqliteContext(databaseConnString ?? "");

            return this;
        }
    }
}