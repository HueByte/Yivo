using Discord.WebSocket;

namespace Yivo.Core.Services.ServerInteraction;

public interface IServerInteractionService
{
    Task SendJoinMessageAsync(SocketGuild guild);
}