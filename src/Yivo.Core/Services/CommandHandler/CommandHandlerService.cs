using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Yivo.Core.Logic;

namespace Yivo.Core.Services.CommandHandler;

public class CommandHandlerService : ICommandHandlerService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordShardedClient _discordClient;
    private readonly InteractionService _interactionService;

    public CommandHandlerService(ILogger<ICommandHandlerService> logger, IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider, DiscordShardedClient discordClient, InteractionService interactionService)
    {
        _logger = logger;
        _serviceScopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _discordClient = discordClient;
        _interactionService = interactionService;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing Command Handler Service");
        _logger.LogInformation("Adding Modules");
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
    }

    public async Task HandleCommandAsync(SocketInteraction interaction)
    {
        ExtendedShardedInteractionContext? extendedContext = null;
        try
        {
            var scope = _serviceScopeFactory.CreateAsyncScope();
            extendedContext = new ExtendedShardedInteractionContext(_discordClient, interaction, scope);

            await interaction.DeferAsync();
            await _interactionService.ExecuteCommandAsync(extendedContext, scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute command");
            if (extendedContext is not null)
                await extendedContext.DisposeAsync();
        }
    }

    public async Task HandleSlashCommandExecutedAsync(SlashCommandInfo slashCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        var extendedContext = interactionContext as ExtendedShardedInteractionContext;
        var scope = extendedContext is not null ? extendedContext.AsyncScope : _serviceScopeFactory.CreateAsyncScope();
        try
        {
            if (!result.IsSuccess)
            {
                var embed = new EmbedBuilder().WithCurrentTimestamp()
                                              .WithColor(Color.Red);
                //   .WithThumbnailUrl(Icons.Error);
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        embed.WithTitle("Unmet Precondition");
                        embed.WithDescription(result.ErrorReason);
                        break;

                    case InteractionCommandError.UnknownCommand:
                        embed.WithTitle("Unknown command");
                        embed.WithDescription(result.ErrorReason);
                        break;

                    case InteractionCommandError.BadArgs:
                        embed.WithTitle($"Invalid number or arguments");
                        embed.WithDescription(result.ErrorReason);
                        break;

                    case InteractionCommandError.Exception:
                        embed.WithTitle("Command exception");
                        embed.WithDescription(result.ErrorReason);
                        break;

                    case InteractionCommandError.Unsuccessful:
                        embed.WithTitle("Command could not be executed");
                        embed.WithDescription(result.ErrorReason);
                        break;

                    default:
                        embed.WithTitle("Something went wrong");
                        embed.WithDescription(result.ErrorReason);
                        break;
                }

                await interactionContext.Interaction.ModifyOriginalResponseAsync((msg) => msg.Embed = embed.Build());
            }
        }
        finally
        {
            if (extendedContext is null) scope.Dispose();
            else await scope.DisposeAsync();
        }
    }
}