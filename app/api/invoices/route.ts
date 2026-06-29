import { NextRequest, NextResponse } from 'next/server';

/**
 * GET  /api/invoices       → Liste toutes les factures du user
 * POST /api/invoices       → Créer une facture depuis dashboard
 *
 * Le webhook WhatsApp Bot est dans /api/webhook/route.ts (à créer séparément)
 */
export async function GET(req: NextRequest) {
  // TODO: Récupérer depuis Cosmos DB par userId (JWT token)
  return NextResponse.json({ invoices: [] });
}

export async function POST(req: NextRequest) {
  const body = await req.json();
  // TODO: Valider, sauvegarder, générer PDF via QuestPDF (.NET), envoyer sur WhatsApp
  return NextResponse.json({ success: true, invoice: body });
}
