/**
 * Client HTTP typé vers l'API .NET (apps/api).
 *
 * Les formes renvoyées correspondent exactement aux types de `./types`
 * (propriétés camelCase, enums en snake_case côté serveur — contrat aligné).
 *
 * Auth : si Clerk est actif (ClerkProvider chargé), on attache le jeton de
 * session en Bearer. L'API résout alors le tenant depuis le token (org_id).
 * Sans Clerk, aucun en-tête → l'API retombe sur le tenant de démo.
 */

import type { ClientCompletion, DashboardStats, EndClient, Piece, Tenant } from './types';

/** Base de l'API .NET. Surcharger via NEXT_PUBLIC_API_URL (défaut = profil http dev). */
export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, '') ?? 'http://localhost:5221';

export class ApiError extends Error {
  constructor(
    message: string,
    readonly status: number,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

interface ClerkGlobal {
  session?: { getToken: () => Promise<string | null> };
}

/** En-tête Authorization (Bearer) si une session Clerk est présente côté navigateur. */
async function authHeaders(): Promise<Record<string, string>> {
  if (typeof window === 'undefined') return {};
  const clerk = (window as unknown as { Clerk?: ClerkGlobal }).Clerk;
  if (!clerk?.session) return {};
  try {
    const token = await clerk.session.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  } catch {
    return {};
  }
}

async function getJson<T>(path: string, signal?: AbortSignal): Promise<T> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    headers: { Accept: 'application/json', ...(await authHeaders()) },
    signal,
  });
  if (!res.ok) {
    throw new ApiError(`GET ${path} → ${res.status}`, res.status);
  }
  return (await res.json()) as T;
}

async function sendJson<T>(method: 'POST' | 'PUT', path: string, body: unknown): Promise<T> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
      ...(await authHeaders()),
    },
    body: JSON.stringify(body),
  });
  if (!res.ok) {
    let message = `${method} ${path} → ${res.status}`;
    try {
      const err = await res.json();
      if (err?.error) message = err.error as string;
    } catch {
      /* corps non-JSON */
    }
    throw new ApiError(message, res.status);
  }
  return (await res.json()) as T;
}

async function deleteResource(path: string): Promise<void> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    method: 'DELETE',
    headers: { ...(await authHeaders()) },
  });
  if (!res.ok && res.status !== 204) {
    throw new ApiError(`DELETE ${path} → ${res.status}`, res.status);
  }
}

interface ListEnvelope<T> {
  items: T[];
  total?: number;
}

export function fetchDashboardStats(month: string, signal?: AbortSignal): Promise<DashboardStats> {
  return getJson<DashboardStats>(`/api/stats/dashboard?month=${encodeURIComponent(month)}`, signal);
}

export async function fetchClientCompletions(
  month: string,
  signal?: AbortSignal,
): Promise<ClientCompletion[]> {
  const data = await getJson<{ month: string; items: ClientCompletion[] }>(
    `/api/stats/completion?month=${encodeURIComponent(month)}`,
    signal,
  );
  return data.items;
}

export async function fetchPieces(month: string, signal?: AbortSignal): Promise<Piece[]> {
  const data = await getJson<ListEnvelope<Piece>>(
    `/api/pieces?month=${encodeURIComponent(month)}`,
    signal,
  );
  return data.items;
}

export async function fetchEndClients(search?: string, signal?: AbortSignal): Promise<EndClient[]> {
  const query = search ? `?search=${encodeURIComponent(search)}` : '';
  const data = await getJson<ListEnvelope<EndClient>>(`/api/endclients${query}`, signal);
  return data.items;
}

export function fetchCurrentTenant(signal?: AbortSignal): Promise<Tenant> {
  return getJson<Tenant>('/api/tenants/me', signal);
}

/** Groupe les pièces par client (calcul côté frontend, comme l'inbox du dashboard). */
export function groupPiecesByClient(
  clients: EndClient[],
  pieces: Piece[],
): Array<{ client: EndClient; pieces: Piece[] }> {
  return clients
    .map((client) => ({
      client,
      pieces: pieces
        .filter((p) => p.endClientId === client.id)
        .sort((a, b) => b.receivedAt.localeCompare(a.receivedAt)),
    }))
    .filter((group) => group.pieces.length > 0);
}

// ── Mutations EndClients (spec §6.3) ────────────────────────────────────────

/** Corps de création/édition d'un client (aligné sur CreateEndClientRequest .NET). */
export interface EndClientInput {
  companyName: string;
  contactName: string;
  whatsappNumber: string;
  email?: string;
  siret?: string;
  vatNumber?: string;
  tags?: string[];
}

export function createEndClient(input: EndClientInput): Promise<EndClient> {
  return sendJson<EndClient>('POST', '/api/endclients', input);
}

export function updateEndClient(id: string, input: EndClientInput): Promise<EndClient> {
  return sendJson<EndClient>('PUT', `/api/endclients/${id}`, input);
}

export function deleteEndClient(id: string): Promise<void> {
  return deleteResource(`/api/endclients/${id}`);
}

export function fetchEndClient(id: string, signal?: AbortSignal): Promise<EndClient> {
  return getJson<EndClient>(`/api/endclients/${id}`, signal);
}

// ── Mutations Pièces (spec §6.4) ────────────────────────────────────────────

/** Valide l'extraction d'une pièce (passe en statut validated). */
export function validatePiece(id: string): Promise<Piece> {
  return sendJson<Piece>('PUT', `/api/pieces/${id}/validate`, {});
}

/** Corrige la catégorie et/ou les données extraites d'une pièce. */
export function correctPiece(
  id: string,
  patch: { category?: Piece['category']; extractedData?: Partial<Piece['extractedData']> },
): Promise<Piece> {
  return sendJson<Piece>('PUT', `/api/pieces/${id}`, patch);
}
