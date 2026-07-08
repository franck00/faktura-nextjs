import { clerkMiddleware } from '@clerk/nextjs/server';
import { NextResponse } from 'next/server';

/**
 * Auth Clerk (spec §2.3) — activée uniquement si la clé publishable est présente.
 * Sans clé (dev / démo), le middleware laisse passer toutes les requêtes, donc
 * l'app fonctionne sans configurer Clerk. Pour activer : renseigner
 * NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY + CLERK_SECRET_KEY dans .env.local.
 */
const clerkEnabled = Boolean(process.env.NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY);

export default clerkEnabled ? clerkMiddleware() : () => NextResponse.next();

export const config = {
  matcher: [
    // Ignore les fichiers statiques et les internes Next, sauf si présents dans un search param.
    '/((?!_next|[^?]*\\.(?:html?|css|js(?!on)|jpe?g|webp|png|gif|svg|ttf|woff2?|ico|csv|docx?|xlsx?|zip|webmanifest)).*)',
    // Toujours passer sur les routes API.
    '/(api|trpc)(.*)',
  ],
};
