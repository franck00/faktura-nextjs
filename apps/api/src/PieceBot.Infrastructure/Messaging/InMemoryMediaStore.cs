using System.Collections.Concurrent;
using PieceBot.Core.Abstractions;
using PieceBot.Core.Domain;

namespace PieceBot.Infrastructure.Messaging;

/// <summary>
/// Stockage In-Memory des binaires de pièces (dev / démo). À remplacer par Azure
/// Blob Storage sans impact sur la BLL. Les fichiers sont perdus au redémarrage.
/// </summary>
public sealed class InMemoryMediaStore : IMediaStore
{
    private readonly ConcurrentDictionary<string, MediaFile> _files = new();

    public Task SaveAsync(string key, byte[] bytes, string contentType, string fileName, CancellationToken cancellationToken = default)
    {
        _files[key] = new MediaFile(bytes, contentType, fileName);
        return Task.CompletedTask;
    }

    public Task<MediaFile?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        _files.TryGetValue(key, out var file);
        return Task.FromResult(file);
    }
}
