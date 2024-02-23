using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yivo.Core.Options;
using Yivo.Core.Services.CommandHandler;
using Yivo.Core.Services.EventHandler;
using Yivo.Infrastructure;

namespace Yivo.Bot;

public class YivoHost : IHostedService
{
    private readonly IEventHandlerService _eventHandlerService;
    private readonly DiscordShardedClient _discordClient;
    private readonly InteractionService _interactionService;
    private readonly ILogger<YivoHost> _logger;
    private readonly ICommandHandlerService _commandHandlerService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly BotOptions _botOptions;
    private bool _isInitialized = false;

    public YivoHost(IEventHandlerService eventHandlerService, DiscordShardedClient discordClient, InteractionService interactionService, ILogger<YivoHost> logger, IOptions<BotOptions> botOptions, ICommandHandlerService commandHandlerService, IServiceScopeFactory serviceScopeFactory)
    {
        _eventHandlerService = eventHandlerService;
        _discordClient = discordClient;
        _interactionService = interactionService;
        _logger = logger;
        _commandHandlerService = commandHandlerService;
        _botOptions = botOptions.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateDatabaseAsync();
        await _commandHandlerService.InitializeAsync();

        ConfigureEvents();
        await CreateBotAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Yivo Bot");
        await _discordClient.StopAsync();
    }

    private void ConfigureEvents()
    {
        _logger.LogInformation("Attaching events");

        _discordClient.ShardReady += OnShardReadyAsync;
        _discordClient.ShardReady += _eventHandlerService.OnShardReadyAsync;
        _discordClient.InteractionCreated += _eventHandlerService.OnCommandCreatedAsync;
        _discordClient.Log += _eventHandlerService.OnClientLogAsync;
        _discordClient.JoinedGuild += _eventHandlerService.OnBotJoinedAsync;
        _interactionService.SlashCommandExecuted += _eventHandlerService.OnShashCommandExecutedAsync;
    }

    private async Task OnShardReadyAsync(DiscordSocketClient shard)
    {
        if (_isInitialized) return;

        _logger.LogInformation("Registering slash commands");
        await _interactionService.RegisterCommandsGloballyAsync();

        _isInitialized = true;
    }

    private async Task CreateBotAsync()
    {
        _logger.LogInformation("Starting Yivo Bot");

        await _discordClient.LoginAsync(Discord.TokenType.Bot, _botOptions.Token);
        await _discordClient.StartAsync();
    }

    private async Task CreateDatabaseAsync()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<YivoContext>();

        await dbContext.Database.MigrateAsync();
    }
}