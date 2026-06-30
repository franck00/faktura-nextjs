import { NextRequest, NextResponse } from 'next/server';
import { deleteClient, getClient, updateClient } from '@/lib/piecebot/store';
import { endClientUpdateSchema } from '@/lib/piecebot/validation';

/**
 * GET    /api/endclients/{id} → Détail (spec §6.3)
 * PUT    /api/endclients/{id} → Mise à jour partielle
 * DELETE /api/endclients/{id} → Suppression
 */
export async function GET(_req: NextRequest, { params }: { params: { id: string } }) {
  const client = getClient(params.id);
  if (!client) return NextResponse.json({ error: 'Client introuvable' }, { status: 404 });
  return NextResponse.json(client);
}

export async function PUT(req: NextRequest, { params }: { params: { id: string } }) {
  let body: unknown;
  try {
    body = await req.json();
  } catch {
    return NextResponse.json({ error: 'Corps JSON invalide' }, { status: 400 });
  }

  const parsed = endClientUpdateSchema.safeParse(body);
  if (!parsed.success) {
    return NextResponse.json(
      { error: 'Validation échouée', issues: parsed.error.flatten() },
      { status: 422 },
    );
  }

  const updated = updateClient(params.id, parsed.data);
  if (!updated) return NextResponse.json({ error: 'Client introuvable' }, { status: 404 });
  return NextResponse.json(updated);
}

export async function DELETE(_req: NextRequest, { params }: { params: { id: string } }) {
  const ok = deleteClient(params.id);
  if (!ok) return NextResponse.json({ error: 'Client introuvable' }, { status: 404 });
  return new NextResponse(null, { status: 204 });
}
