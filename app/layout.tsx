import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'PieceBot — Collecte WhatsApp des pièces comptables',
  description:
    'Vos clients envoient leurs justificatifs par WhatsApp. PieceBot les classe automatiquement par client, prêts pour la compta.',
  keywords: ['comptabilité', 'whatsapp', 'afrique', 'cabinet', 'pièces', 'ocr', 'bot'],
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="fr" data-theme="light">
      <body>{children}</body>
    </html>
  );
}
