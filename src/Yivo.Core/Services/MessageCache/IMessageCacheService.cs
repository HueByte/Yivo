using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yivo.Core.Services.MessageCache;

public interface IMessageCacheService
{
    Task<string?> GetServerJoinMessage();
}