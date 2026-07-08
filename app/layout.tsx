import type { Metadata } from 'next';
import { ClerkProvider } from '@clerk/nextjs';
import './globals.css';

export const metadata: Metadata = {
  title: 'PieceBot — Collecte WhatsApp des pièces comptables',
  description:
    'Vos clients envoient leurs justificatifs par WhatsApp. PieceBot les classe automatiquement par client, prêts pour la compta.',
  keywords: ['comptabilité', 'whatsapp', 'afrique', 'cabinet', 'pièces', 'ocr', 'bot'],
};

// Clerk n'enrobe l'app que si configuré (voir middleware.ts) : sans clé, l'app
// tourne en mode démo sans authentification.
const clerkEnabled = Boolean(process.env.NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY);

export default function RootLayout({ children }: { children: React.ReactNode }) {
  const document = (
    <html lang="fr" data-theme="light">
      <body>{children}</body>
    </html>
  );

  return clerkEnabled ? <ClerkProvider>{document}</ClerkProvider> : document;
}
