/**
 * PieceBot — modèle de domaine partagé (frontend).
 *
 * Aligné sur les collections Cosmos DB (spec §4). Les mêmes formes serviront
 * de contrat pour l'API .NET 8 — garder les noms de champs synchronisés avec
 * le spec. Convention : camelCase, montants en nombres entiers (plus petite
 * unité de la devise, ex. XAF sans décimales).
 */

export type CountryCode = 'CM' | 'CI' | 'SN';
export type CurrencyCode = 'XAF' | 'XOF' | 'EUR';

/** Catégorie de pièce justificative reçue. */
export type PieceCategory =
  | 'invoice_purchase' // facture d'achat
  | 'invoice_sale' // facture de vente
  | 'receipt' // ticket / reçu
  | 'bank_statement' // relevé bancaire
  | 'payment_proof' // justificatif de paiement
  | 'other';

/** Cycle de vie d'une pièce, de la réception WhatsApp à la validation. */
export type PieceStatus =
  | 'received' // média stocké, pas encore traité par l'OCR
  | 'processing' // OCR en cours
  | 'extracted' // extraction faite, en attente de validation cabinet
  | 'validated' // validée par le comptable
  | 'rejected' // rejetée (illisible, hors sujet…)
  | 'error'; // échec de traitement

export type SubscriptionStatus = 'trialing' | 'active' | 'past_due' | 'canceled';
export type SubscriptionPlan = 'standard';

/** Mois de rattachement comptable, format `YYYY-MM` (ex. "2026-06"). */
export type AccountingMonth = string;

/** Cabinet comptable = locataire (tenant). Collection `tenants`. */
export interface Tenant {
  id: string;
  tenantId: string;
  type: 'tenant';
  name: string;
  country: CountryCode;
  currency: CurrencyCode;
  vatRate: number;
  whatsappPhoneNumberId: string;
  whatsappBusinessAccountId: string;
  subscriptionStatus: SubscriptionStatus;
  subscriptionPlan: SubscriptionPlan;
  stripeCustomerId: string | null;
  trialEndsAt: string | null;
  createdAt: string;
  owner: {
    userId: string;
    email: string;
    fullName: string;
  };
  settings: {
    language: 'fr' | 'en';
    timezone: string;
    monthlyReminderDay: number;
    monthlyReminderHour: number;
  };
}

/** Entreprise cliente du cabinet. Collection `endClients`. */
export interface EndClient {
  id: string;
  tenantId: string;
  type: 'endclient';
  companyName: string;
  contactName: string;
  whatsappNumber: string;
  email: string | null;
  siret: string | null;
  vatNumber: string | null;
  tags: string[];
  activeMonth: AccountingMonth;
  createdAt: string;
  lastPieceReceived: string | null;
}

/** Données structurées extraites par l'OCR (Document Intelligence). */
export interface ExtractedData {
  supplier: string | null;
  documentDate: string | null;
  documentNumber: string | null;
  totalAmountTtc: number | null;
  totalAmountHt: number | null;
  totalVat: number | null;
  currency: CurrencyCode | null;
  items: ExtractedLineItem[];
}

export interface ExtractedLineItem {
  description: string;
  quantity: number | null;
  unitPrice: number | null;
  amount: number | null;
}

/** Pièce justificative reçue. Collection `pieces`. */
export interface Piece {
  id: string;
  tenantId: string;
  endClientId: string;
  type: 'piece';
  category: PieceCategory;
  status: PieceStatus;
  month: AccountingMonth;
  receivedAt: string;
  blobUrl: string;
  mimeType: string;
  originalFileName: string;
  extractedData: ExtractedData;
  /** Score de confiance global de l'extraction, 0..1. */
  confidence: number;
  validatedBy: string | null;
  validatedAt: string | null;
  ocrRawText: string | null;
  whatsappMessageId: string;
}

/** KPIs du dashboard pour un mois donné (spec §5.5 / §6.6). */
export interface DashboardStats {
  month: AccountingMonth;
  totalPieces: number;
  pendingValidation: number;
  validatedPieces: number;
  activeClients: number;
  /** Taux de complétion global du mois, 0..1. */
  completionRate: number;
}

/** Avancement d'un client sur le mois courant. */
export interface ClientCompletion {
  endClientId: string;
  companyName: string;
  piecesReceived: number;
  piecesValidated: number;
  lastPieceReceived: string | null;
  /** 0..1 — part des pièces validées sur les pièces reçues. */
  completionRate: number;
}
