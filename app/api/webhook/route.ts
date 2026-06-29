import { NextRequest, NextResponse } from 'next/server';

/**
 * WhatsApp Cloud API Webhook
 * GET  → Vérification Meta (challenge)
 * POST → Réception des messages entrants
 *
 * Ce endpoint est appelé par Meta à chaque message reçu sur ton numéro WhatsApp.
 * La logique métier (NLP + génération PDF) tourne idéalement sur Azure Functions (.NET 8).
 * Tu peux aussi faire un proxy ici vers ton backend Azure.
 */

const VERIFY_TOKEN = process.env.META_WEBHOOK_VERIFY_TOKEN ?? 'faktura_verify';

export async function GET(req: NextRequest) {
  const { searchParams } = new URL(req.url);
  const mode      = searchParams.get('hub.mode');
  const token     = searchParams.get('hub.verify_token');
  const challenge = searchParams.get('hub.challenge');

  if (mode === 'subscribe' && token === VERIFY_TOKEN) {
    console.log('[Webhook] Verified');
    return new Response(challenge, { status: 200 });
  }
  return NextResponse.json({ error: 'Forbidden' }, { status: 403 });
}

export async function POST(req: NextRequest) {
  const body = await req.json();

  // Extraire le message
  const entry   = body?.entry?.[0];
  const changes = entry?.changes?.[0];
  const message = changes?.value?.messages?.[0];

  if (!message) return NextResponse.json({ status: 'no_message' });

  const from = message.from;  // numéro WhatsApp de l'expéditeur
  const text = message.text?.body ?? '';

  console.log(`[Webhook] Message de ${from}: ${text}`);

  // TODO: Envoyer au service .NET 8 sur Azure Functions
  // await fetch(process.env.AZURE_FUNCTIONS_URL + '/api/ProcessMessage', {
  //   method: 'POST',
  //   headers: { 'Content-Type': 'application/json', 'x-functions-key': process.env.AZURE_FUNCTIONS_KEY! },
  //   body: JSON.stringify({ from, text }),
  // });

  return NextResponse.json({ status: 'ok' });
}
