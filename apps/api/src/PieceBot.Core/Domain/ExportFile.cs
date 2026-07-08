namespace PieceBot.Core.Domain;

/// <summary>
/// Fichier d'export généré en mémoire (PDF ou Excel), prêt à être stocké puis
/// téléchargé. Découplé du moteur de rendu (QuestPDF / ClosedXML côté infra).
/// </summary>
public sealed record ExportFile(byte[] Bytes, string FileName, string ContentType);
