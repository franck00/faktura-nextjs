/**
 * Schémas Zod pour les EndClients (entrées API/formulaires).
 *
 * Source de vérité de la validation côté Next.js. La même forme sera reprise
 * par l'API .NET (spec §6.3). On valide les entrées utilisateur, pas les
 * documents Cosmos complets (id/tenantId/timestamps sont posés côté serveur).
 */

import { z } from 'zod';

/** Numéro WhatsApp au format international E.164, ex. +237699123456. */
const whatsappNumber = z
  .string()
  .trim()
  .regex(/^\+[1-9]\d{7,14}$/, 'Numéro WhatsApp invalide (format +237699123456)');

/** Entrée de création/édition d'un client du cabinet. */
export const endClientInputSchema = z.object({
  companyName: z.string().trim().min(2, 'Le nom de l’entreprise est requis').max(120),
  contactName: z.string().trim().min(2, 'Le nom du contact est requis').max(120),
  whatsappNumber,
  email: z
    .string()
    .trim()
    .email('Email invalide')
    .optional()
    .or(z.literal('').transform(() => undefined)),
  siret: z.string().trim().max(40).optional().or(z.literal('').transform(() => undefined)),
  vatNumber: z.string().trim().max(40).optional().or(z.literal('').transform(() => undefined)),
  tags: z.array(z.string().trim().min(1)).max(10).default([]),
});

export type EndClientInput = z.infer<typeof endClientInputSchema>;

/** Édition partielle (PUT) — tous les champs optionnels. */
export const endClientUpdateSchema = endClientInputSchema.partial();

export type EndClientUpdate = z.infer<typeof endClientUpdateSchema>;
