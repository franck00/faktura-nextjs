/**
 * Client HTTP typé vers l'API .NET (apps/api).
 *
 * Les formes renvoyées correspondent exactement aux types de `./types`
 * (propriétés camelCase, enums en snake_case côté serveur — contrat aligné).
 * L'auth Clerk n'est pas encore branchée : l'API résout un tenant constant.
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

async function getJson<T>(path: string, signal?: AbortSignal): Promise<T> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    headers: { Accept: 'application/json' },
    signal,
  });
  if (!res.ok) {
    throw new ApiError(`GET ${path} → ${res.status}`, res.status);
  }
  return (await res.json()) as T;
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
