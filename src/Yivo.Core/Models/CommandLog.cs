using System.ComponentModel.DataAnnotations.Schema;
using Yivo.Core.Abstraction;

namespace Yivo.Core.Models;

public class CommandLog : DbModel<ulong>
{
    public DateTime Date { get; set; }
    public int ExecutionTime { get; set; }
    public string CommandName { get; set; } = default!;

    [ForeignKey("UserId")]
    public ulong UserId { get; set; }
    public virtual User? User { get; set; }

}