using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Yivo.Core.Services.CommandHandler
{
    public interface ICommandHandlerService
    {
        Task InitializeAsync();
        Task HandleCommandAsync(SocketInteraction interaction);
        Task HandleSlashCommandExecutedAsync(SlashCommandInfo slashCommandInfo, IInteractionContext interactionContext, IResult result);
    }
}