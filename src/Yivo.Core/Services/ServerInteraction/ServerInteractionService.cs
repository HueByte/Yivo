using Discord;
using Discord.WebSocket;
using Yivo.Core.Services.MessageCache;

namespace Yivo.Core.Services.ServerInteraction;

public class ServerInteractionService : IServerInteractionService
{
    private readonly IMessageCacheService _messageCacheService;
    public ServerInteractionService(IMessageCacheService messageCacheService)
    {
        _messageCacheService = messageCacheService;
    }

    public async Task SendJoinMessageAsync(SocketGuild guild)
    {
        var joinMessage = await _messageCacheService.GetServerJoinMessage();
        var embed = new EmbedBuilder().WithTitle("✨ Hello I'm Yivo! ✨")
                                     .WithColor(Color.Teal)
                                     .WithDescription(joinMessage)
                                     //  .WithThumbnailUrl()
                                     .WithCurrentTimestamp();

        await guild.DefaultChannel.SendMessageAsync(null, false, embed.Build());
    }
}