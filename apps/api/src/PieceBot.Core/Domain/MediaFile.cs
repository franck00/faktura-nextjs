namespace PieceBot.Core.Domain;

/// <summary>
/// Binaire d'une pièce (photo/PDF) stocké et servi par l'API. Découplé du support
/// de stockage (In-Memory aujourd'hui, Azure Blob demain).
/// </summary>
public sealed record MediaFile(byte[] Bytes, string ContentType, string FileName);
