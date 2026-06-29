import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Faktura — Facturation WhatsApp pour l'Afrique',
  description: 'Un message WhatsApp. Un PDF professionnel. En moins de 15 secondes.',
  keywords: ['facture', 'whatsapp', 'afrique', 'pme', 'invoice', 'bot'],
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="fr" data-theme="light">
      <body>{children}</body>
    </html>
  );
}
