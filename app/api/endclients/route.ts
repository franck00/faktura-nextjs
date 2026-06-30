import { NextRequest, NextResponse } from 'next/server';
import { createClient, listClients } from '@/lib/piecebot/store';
import { endClientInputSchema } from '@/lib/piecebot/validation';

/**
 * GET  /api/endclients?search=  → Liste des clients du cabinet (spec §6.3)
 * POST /api/endclients          → Créer un client
 *
 * Provisoirement adossé au store mémoire — à remplacer par Cosmos DB,
 * partition tenantId (auth Clerk à brancher pour résoudre le tenant).
 */
export async function GET(req: NextRequest) {
  const search = req.nextUrl.searchParams.get('search') ?? undefined;
  const items = listClients(search);
  return NextResponse.json({ items, total: items.length });
}

export async function POST(req: NextRequest) {
  let body: unknown;
  try {
    body = await req.json();
  } catch {
    return NextResponse.json({ error: 'Corps JSON invalide' }, { status: 400 });
  }

  const parsed = endClientInputSchema.safeParse(body);
  if (!parsed.success) {
    return NextResponse.json(
      { error: 'Validation échouée', issues: parsed.error.flatten() },
      { status: 422 },
    );
  }

  const client = createClient(parsed.data);
  return NextResponse.json(client, { status: 201 });
}
