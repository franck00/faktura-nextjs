/**
 * Données de démonstration PieceBot + sélecteurs dérivés.
 *
 * Sert tant qu'il n'y a pas d'API .NET / Cosmos branchée. Les sélecteurs
 * (`getDashboardStats`, `getClientCompletions`, `getPiecesByClient`)
 * reproduisent ce que renverront les endpoints `/api/stats` et `/api/pieces`,
 * pour que les composants soient déjà écrits contre les bonnes formes.
 */

import type {
  ClientCompletion,
  DashboardStats,
  EndClient,
  Piece,
  Tenant,
} from './types';

export const CURRENT_MONTH = '2026-06';

export const DEMO_TENANT: Tenant = {
  id: 'tenant_mvogo',
  tenantId: 'tenant_mvogo',
  type: 'tenant',
  name: 'Cabinet Mvogo & Associés',
  country: 'CM',
  currency: 'XAF',
  vatRate: 19.25,
  whatsappPhoneNumberId: '1234567890',
  whatsappBusinessAccountId: '9876543210',
  subscriptionStatus: 'trialing',
  subscriptionPlan: 'standard',
  stripeCustomerId: null,
  trialEndsAt: '2026-07-13T00:00:00Z',
  createdAt: '2026-06-29T10:00:00Z',
  owner: {
    userId: 'clerk_user_id',
    email: 'jean@cabinet-mvogo.cm',
    fullName: 'Jean Mvogo',
  },
  settings: {
    language: 'fr',
    timezone: 'Africa/Douala',
    monthlyReminderDay: 5,
    monthlyReminderHour: 9,
  },
};

export const DEMO_END_CLIENTS: EndClient[] = [
  {
    id: 'client_aissa',
    tenantId: 'tenant_mvogo',
    type: 'endclient',
    companyName: 'SARL Boutique Aïssa',
    contactName: 'Aïssa Diallo',
    whatsappNumber: '+237699123456',
    email: 'aissa@boutique.cm',
    siret: 'M032018123456',
    vatNumber: 'P123456789',
    tags: ['commerce', 'actif'],
    activeMonth: CURRENT_MONTH,
    createdAt: '2026-05-02T10:00:00Z',
    lastPieceReceived: '2026-06-28T15:32:00Z',
  },
  {
    id: 'client_batipro',
    tenantId: 'tenant_mvogo',
    type: 'endclient',
    companyName: 'BatiPro Douala',
    contactName: 'Serge Nkomo',
    whatsappNumber: '+237677889900',
    email: 'serge@batipro.cm',
    siret: 'M052019998877',
    vatNumber: 'P987654321',
    tags: ['btp'],
    activeMonth: CURRENT_MONTH,
    createdAt: '2026-04-18T10:00:00Z',
    lastPieceReceived: '2026-06-27T09:14:00Z',
  },
  {
    id: 'client_pharma',
    tenantId: 'tenant_mvogo',
    type: 'endclient',
    companyName: 'Pharmacie du Wouri',
    contactName: 'Dr. Mballa',
    whatsappNumber: '+237698112233',
    email: null,
    siret: 'M012017445566',
    vatNumber: null,
    tags: ['sante'],
    activeMonth: CURRENT_MONTH,
    createdAt: '2026-03-10T10:00:00Z',
    lastPieceReceived: '2026-06-10T11:02:00Z',
  },
  {
    id: 'client_transport',
    tenantId: 'tenant_mvogo',
    type: 'endclient',
    companyName: 'Transports Étoile',
    contactName: 'Paul Etoga',
    whatsappNumber: '+237691445566',
    email: 'paul@etoile-transport.cm',
    siret: 'M072020334455',
    vatNumber: 'P456789123',
    tags: ['transport', 'retard'],
    activeMonth: CURRENT_MONTH,
    createdAt: '2026-05-20T10:00:00Z',
    lastPieceReceived: null,
  },
];

function blob(id: string, ext = 'jpg') {
  return `https://piecebot.blob.core.windows.net/pieces/tenant_mvogo/${CURRENT_MONTH}/${id}.${ext}`;
}

export const DEMO_PIECES: Piece[] = [
  {
    id: 'piece_001',
    tenantId: 'tenant_mvogo',
    endClientId: 'client_aissa',
    type: 'piece',
    category: 'invoice_purchase',
    status: 'extracted',
    month: CURRENT_MONTH,
    receivedAt: '2026-06-28T15:32:00Z',
    blobUrl: blob('piece_001'),
    mimeType: 'image/jpeg',
    originalFileName: 'IMG_20260628.jpg',
    extractedData: {
      supplier: 'ENEO Cameroun',
      documentDate: '2026-06-15',
      documentNumber: 'FAC-2026-08-12345',
      totalAmountTtc: 78500,
      totalAmountHt: 65798,
      totalVat: 12702,
      currency: 'XAF',
      items: [],
    },
    confidence: 0.92,
    validatedBy: null,
    validatedAt: null,
    ocrRawText: 'ENEO CAMEROUN — Facture electricite ...',
    whatsappMessageId: 'wamid.aaa',
  },
  {
    id: 'piece_002',
    tenantId: 'tenant_mvogo',
    endClientId: 'client_aissa',
    type: 'piece',
    category: 'receipt',
    status: 'extracted',
    month: CURRENT_MONTH,
    receivedAt: '2026-06-26T08:10:00Z',
    blobUrl: blob('piece_002'),
    mimeType: 'image/jpeg',
    originalFileName: 'ticket_marche.jpg',
    extractedData: {
      supplier: 'Marché Mboppi',
      documentDate: '2026-06-25',
      documentNumber: null,
      totalAmountTtc: 24000,
      totalAmountHt: null,
      totalVat: null,
      currency: 'XAF',
      items: [],
    },
    confidence: 0.61,
    validatedBy: null,
    validatedAt: null,
    ocrRawText: 'TICKET ...',
    whatsappMessageId: 'wamid.bbb',
  },
  {
    id: 'piece_003',
    tenantId: 'tenant_mvogo',
    endClientId: 'client_aissa',
    type: 'piece',
    category: 'invoice_purchase',
    status: 'validated',
    month: CURRENT_MONTH,
    receivedAt: '2026-06-12T14:00:00Z',
    blobUrl: blob('piece_003', 'pdf'),
    mimeType: 'application/pdf',
    originalFileName: 'facture_camtel.pdf',
    extractedData: {
      supplier: 'CAMTEL',
      documentDate: '2026-06-05',
      documentNumber: 'CT-556677',
      totalAmountTtc: 45000,
      totalAmountHt: 37735,
      totalVat: 7265,
      currency: 'XAF',
      items: [],
    },
    confidence: 0.97,
    validatedBy: 'clerk_user_id',
    validatedAt: '2026-06-13T09:20:00Z',
    ocrRawText: 'CAMTEL ...',
    whatsappMessageId: 'wamid.ccc',
  },
  {
    id: 'piece_004',
    tenantId: 'tenant_mvogo',
    endClientId: 'client_batipro',
    type: 'piece',
    category: 'invoice_purchase',
    status: 'extracted',
    month: CURRENT_MONTH,
    receivedAt: '2026-06-27T09:14:00Z',
    blobUrl: blob('piece_004'),
    mimeType: 'image/jpeg',
    originalFileName: 'ciment.jpg',
    extractedData: {
      supplier: 'Cimencam',
      documentDate: '2026-06-24',
      documentNumber: 'CIM-2026-4521',
      totalAmountTtc: 1250000,
      totalAmountHt: 1048218,
      totalVat: 201782,
      currency: 'XAF',
      items: [],
    },
    confidence: 0.88,
    validatedBy: null,
    validatedAt: null,
    ocrRawText: 'CIMENCAM ...',
    whatsappMessageId: 'wamid.ddd',
  },
  {
    id: 'piece_005',
    tenantId: 'tenant_mvogo',
    endClientId: 'client_batipro',
    type: 'piece',
    category: 'bank_statement',
    status: 'received',
    month: CURRENT_MONTH,
    receivedAt: '2026-06-27T09:16:00Z',
    blobUrl: blob('piece_005', 'pdf'),
    mimeType: 'application/pdf',
    originalFileName: 'releve_afriland.pdf',
    extractedData: {
      supplier: null,
      documentDate: null,
      documentNumber: null,
      totalAmountTtc: null,
      totalAmountHt: null,
      totalVat: null,
      currency: null,
      items: [],
    },
    confidence: 0,
    validatedBy: null,
    validatedAt: null,
    ocrRawText: null,
    whatsappMessageId: 'wamid.eee',
  },
  {
    id: 'piece_006',
    tenantId: 'tenant_mvogo',
    endClientId: 'client_pharma',
    type: 'piece',
    category: 'invoice_purchase',
    status: 'validated',
    month: CURRENT_MONTH,
    receivedAt: '2026-06-10T11:02:00Z',
    blobUrl: blob('piece_006'),
    mimeType: 'image/jpeg',
    originalFileName: 'fournisseur_pharma.jpg',
    extractedData: {
      supplier: 'Laborex Cameroun',
      documentDate: '2026-06-08',
      documentNumber: 'LBX-99812',
      totalAmountTtc: 890000,
      totalAmountHt: 746331,
      totalVat: 143669,
      currency: 'XAF',
      items: [],
    },
    confidence: 0.94,
    validatedBy: 'clerk_user_id',
    validatedAt: '2026-06-11T08:00:00Z',
    ocrRawText: 'LABOREX ...',
    whatsappMessageId: 'wamid.fff',
  },
];

// ── Sélecteurs (futurs endpoints API) ──────────────────────────────────────

export function getPiecesForMonth(month: string, pieces = DEMO_PIECES): Piece[] {
  return pieces.filter((p) => p.month === month);
}

export function getDashboardStats(
  month: string,
  pieces = DEMO_PIECES,
  clients = DEMO_END_CLIENTS,
): DashboardStats {
  const monthPieces = getPiecesForMonth(month, pieces);
  const validated = monthPieces.filter((p) => p.status === 'validated').length;
  const pending = monthPieces.filter(
    (p) => p.status === 'extracted' || p.status === 'received' || p.status === 'processing',
  ).length;
  const activeClients = new Set(monthPieces.map((p) => p.endClientId)).size;

  return {
    month,
    totalPieces: monthPieces.length,
    pendingValidation: pending,
    validatedPieces: validated,
    activeClients,
    completionRate: monthPieces.length === 0 ? 0 : validated / monthPieces.length,
  };
}

export function getClientCompletions(
  month: string,
  pieces = DEMO_PIECES,
  clients = DEMO_END_CLIENTS,
): ClientCompletion[] {
  return clients.map((c) => {
    const received = pieces.filter((p) => p.endClientId === c.id && p.month === month);
    const validated = received.filter((p) => p.status === 'validated').length;
    return {
      endClientId: c.id,
      companyName: c.companyName,
      piecesReceived: received.length,
      piecesValidated: validated,
      lastPieceReceived: c.lastPieceReceived,
      completionRate: received.length === 0 ? 0 : validated / received.length,
    };
  });
}

/** Pièces groupées par client pour la vue inbox. */
export function getPiecesByClient(
  month: string,
  pieces = DEMO_PIECES,
  clients = DEMO_END_CLIENTS,
): Array<{ client: EndClient; pieces: Piece[] }> {
  return clients
    .map((client) => ({
      client,
      pieces: pieces
        .filter((p) => p.endClientId === client.id && p.month === month)
        .sort((a, b) => b.receivedAt.localeCompare(a.receivedAt)),
    }))
    .filter((group) => group.pieces.length > 0);
}
