using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CorePass.Core.Model;

public record EntryIndex(Guid EntryId, byte[] Nonce, byte[] Cipher);

public record VaultDocument(
    List<EntryIndex> Entries,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc,
    bool ObfuscateNames)
{
    public static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public record EntryPayload(
    string Title,
    string Username,
    string Password,
    string? Url,
    string? Notes,
    List<string>? Tags,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc);
