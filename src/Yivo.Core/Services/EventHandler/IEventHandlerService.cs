using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Yivo.Core.Services.EventHandler;

public interface IEventHandlerService
{
    Task OnShardReadyAsync(DiscordSocketClient shard);
    Task OnCommandCreatedAsync(SocketInteraction interaction);
    Task OnShashCommandExecutedAsync(SlashCommandInfo slashCommandInfo, IInteractionContext interactionContext, IResult result);
    Task OnClientLogAsync(LogMessage logMessage);
    Task OnBotJoinedAsync(SocketGuild guild);
}