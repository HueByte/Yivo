using Discord.Interactions;
using Yivo.Core.Logic;

namespace Yivo.Bot.Commands
{
    [Group("dev", "Developer commands")]
    public class DevCommands : InteractionModuleBase<ExtendedShardedInteractionContext>
    {
        [SlashCommand("ping", "Pong!")]
        public async Task PingAsync()
        {
            await ModifyOriginalResponseAsync((msg) => msg.Content = "Pong!");
        }
    }
}