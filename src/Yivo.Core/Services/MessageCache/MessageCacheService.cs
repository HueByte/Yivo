using Microsoft.Extensions.Caching.Memory;

namespace Yivo.Core.Services.MessageCache;

public class MessageCacheService : IMessageCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly string MESSAGES_BASE_PATH = Path.Join(AppContext.BaseDirectory, "Messages");

    public MessageCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<string?> GetServerJoinMessage()
    {
        string messageKey = "ServerJoinMessage";
        return await GetMessageAsync(messageKey);
    }

    private async Task<string?> GetMessageAsync(string key)
    {
        if (_memoryCache.TryGetValue(key, out string? message))
        {
            return message;
        }
        else if (File.Exists(Path.Join(MESSAGES_BASE_PATH, $"{key}.md")))
        {
            message = await File.ReadAllTextAsync(Path.Join(MESSAGES_BASE_PATH, $"{key}.md"));
            _memoryCache.Set(key, message, TimeSpan.FromMinutes(5));
            return message;
        }

        return null;
    }
}