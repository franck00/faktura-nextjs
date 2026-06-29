import { NextRequest, NextResponse } from 'next/server';

/**
 * POST /api/waitlist
 * Body: { email: string }
 *
 * TODO: Connecter à ta base de données (Cosmos DB, Supabase, etc.)
 */
export async function POST(req: NextRequest) {
  try {
    const { email } = await req.json();

    if (!email || !email.includes('@')) {
      return NextResponse.json({ error: 'Email invalide' }, { status: 400 });
    }

    // TODO: Insérer dans Cosmos DB
    // const client = new CosmosClient(process.env.AZURE_COSMOS_CONNECTION_STRING!);
    // const db = client.database('faktura');
    // const container = db.container('waitlist');
    // await container.items.upsert({ id: email, email, createdAt: new Date().toISOString() });

    // TODO: Envoyer email de confirmation (SendGrid, Resend...)

    console.log('[Waitlist] New signup:', email);

    return NextResponse.json({ success: true, email });
  } catch (err) {
    console.error('[Waitlist] Error:', err);
    return NextResponse.json({ error: 'Erreur serveur' }, { status: 500 });
  }
}
