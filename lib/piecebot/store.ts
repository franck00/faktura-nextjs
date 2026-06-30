/**
 * Store EndClients en mémoire (provisoire, en attendant Cosmos DB).
 *
 * Source unique pour les routes /api/endclients. Single-tenant pour l'instant
 * (DEMO_TENANT). À remplacer par un repository Cosmos (spec §4.2) sans toucher
 * à la signature de ces fonctions.
 */

import { CURRENT_MONTH, DEMO_END_CLIENTS, DEMO_TENANT } from './mock';
import type { EndClient } from './types';
import type { EndClientInput, EndClientUpdate } from './validation';

// Copie mutable seedée depuis les données de démo.
let clients: EndClient[] = DEMO_END_CLIENTS.map((c) => ({ ...c }));

export function listClients(search?: string): EndClient[] {
  const sorted = [...clients].sort((a, b) => a.companyName.localeCompare(b.companyName));
  if (!search) return sorted;
  const q = search.trim().toLowerCase();
  return sorted.filter(
    (c) =>
      c.companyName.toLowerCase().includes(q) ||
      c.contactName.toLowerCase().includes(q) ||
      c.whatsappNumber.includes(q) ||
      c.tags.some((t) => t.toLowerCase().includes(q)),
  );
}

export function getClient(id: string): EndClient | undefined {
  return clients.find((c) => c.id === id);
}

export function createClient(input: EndClientInput): EndClient {
  const now = new Date().toISOString();
  const client: EndClient = {
    id: `endclient_${crypto.randomUUID()}`,
    tenantId: DEMO_TENANT.tenantId,
    type: 'endclient',
    companyName: input.companyName,
    contactName: input.contactName,
    whatsappNumber: input.whatsappNumber,
    email: input.email ?? null,
    siret: input.siret ?? null,
    vatNumber: input.vatNumber ?? null,
    tags: input.tags ?? [],
    activeMonth: CURRENT_MONTH,
    createdAt: now,
    lastPieceReceived: null,
  };
  clients = [client, ...clients];
  return client;
}

export function updateClient(id: string, patch: EndClientUpdate): EndClient | undefined {
  const idx = clients.findIndex((c) => c.id === id);
  if (idx === -1) return undefined;
  const current = clients[idx];
  const updated: EndClient = {
    ...current,
    ...(patch.companyName !== undefined && { companyName: patch.companyName }),
    ...(patch.contactName !== undefined && { contactName: patch.contactName }),
    ...(patch.whatsappNumber !== undefined && { whatsappNumber: patch.whatsappNumber }),
    ...(patch.email !== undefined && { email: patch.email ?? null }),
    ...(patch.siret !== undefined && { siret: patch.siret ?? null }),
    ...(patch.vatNumber !== undefined && { vatNumber: patch.vatNumber ?? null }),
    ...(patch.tags !== undefined && { tags: patch.tags }),
  };
  clients[idx] = updated;
  return updated;
}

export function deleteClient(id: string): boolean {
  const before = clients.length;
  clients = clients.filter((c) => c.id !== id);
  return clients.length < before;
}
