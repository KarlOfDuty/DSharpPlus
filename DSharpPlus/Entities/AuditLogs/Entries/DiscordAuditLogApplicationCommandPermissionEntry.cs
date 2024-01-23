using System.Collections.Generic;

namespace DSharpPlus.Entities.AuditLogs;

public sealed class DiscordAuditLogApplicationCommandPermissionEntry : DiscordAuditLogEntry
{
    /// <summary>
    /// Id of the application command that was changed
    /// </summary>
    public ulong? ApplicationCommandId { get; internal set; }

    /// <summary>
    /// Id of the application that owns the command
    /// </summary>
    public ulong ApplicationId { get; internal set; }

    /// <summary>
    /// Permissions changed
    /// </summary>
    public IEnumerable<PropertyChange<DiscordApplicationCommandPermission>> PermissionChanges { get; internal set; } = default!;
}