import { clerkMiddleware, createRouteMatcher } from '@clerk/nextjs/server';
import { NextResponse } from 'next/server';

/**
 * Auth Clerk (spec §2.3) — activée uniquement si la clé publishable est présente.
 * Sans clé (dev / démo), le middleware laisse passer toutes les requêtes, donc
 * l'app fonctionne sans configurer Clerk. Pour activer : renseigner
 * NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY + CLERK_SECRET_KEY dans .env.local.
 *
 * Quand Clerk est actif, le dashboard exige une connexion ; la landing et les
 * pages d'auth restent publiques.
 */
const clerkEnabled = Boolean(process.env.NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY);

const isProtectedRoute = createRouteMatcher(['/dashboard(.*)']);

export default clerkEnabled
  ? clerkMiddleware((auth, req) => {
      if (isProtectedRoute(req)) {
        auth().protect();
      }
    })
  : () => NextResponse.next();

export const config = {
  matcher: [
    // Ignore les fichiers statiques et les internes Next, sauf si présents dans un search param.
    '/((?!_next|[^?]*\\.(?:html?|css|js(?!on)|jpe?g|webp|png|gif|svg|ttf|woff2?|ico|csv|docx?|xlsx?|zip|webmanifest)).*)',
    // Toujours passer sur les routes API.
    '/(api|trpc)(.*)',
  ],
};
