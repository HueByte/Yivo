using Yivo.Core.Abstraction;

namespace Yivo.Core.Models;

public class User : DbModel<ulong>
{
    public string Username { get; set; } = default!;
    public DateTime FirstSeenDate { get; set; }
    public int CommandsExecutedCount { get; set; }
    public virtual List<CommandLog> CommandLogs { get; set; } = new();
}