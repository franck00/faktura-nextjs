/** Helpers d'affichage PieceBot (labels FR + formatage). */

import type { CurrencyCode, PieceCategory, PieceStatus } from './types';

export const CATEGORY_LABELS: Record<PieceCategory, string> = {
  invoice_purchase: "Facture d'achat",
  invoice_sale: 'Facture de vente',
  receipt: 'Ticket / reçu',
  bank_statement: 'Relevé bancaire',
  payment_proof: 'Justificatif de paiement',
  other: 'Autre',
};

interface StatusMeta {
  label: string;
  color: string;
}

export const STATUS_META: Record<PieceStatus, StatusMeta> = {
  received: { label: 'Reçue', color: '#94A3B8' },
  processing: { label: 'Traitement', color: '#3B82F6' },
  extracted: { label: 'À valider', color: '#F59E0B' },
  validated: { label: 'Validée', color: '#25D366' },
  rejected: { label: 'Rejetée', color: '#EF4444' },
  error: { label: 'Erreur', color: '#EF4444' },
};

export function formatAmount(amount: number | null, currency: CurrencyCode | null): string {
  if (amount === null) return '—';
  const formatted = new Intl.NumberFormat('fr-FR').format(amount);
  return currency ? `${formatted} ${currency}` : formatted;
}

export function formatPercent(rate: number): string {
  return `${Math.round(rate * 100)}%`;
}

/** Date courte "28 juin", à partir d'un ISO string. */
export function formatShortDate(iso: string | null): string {
  if (!iso) return '—';
  return new Intl.DateTimeFormat('fr-FR', { day: '2-digit', month: 'short' }).format(new Date(iso));
}

/** Initiales d'un nom d'entreprise (2 lettres max). */
export function initials(name: string): string {
  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((w) => w[0]?.toUpperCase() ?? '')
    .join('');
}

/** Couleur de l'indicateur de confiance OCR. */
export function confidenceColor(confidence: number): string {
  if (confidence >= 0.85) return '#25D366';
  if (confidence >= 0.6) return '#F59E0B';
  return '#EF4444';
}
