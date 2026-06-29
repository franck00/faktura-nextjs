# Faktura — WhatsApp Invoice Bot

> Facturation WhatsApp pour les PME africaines. Un message naturel → un PDF pro en < 15 secondes.

## Stack

- **Next.js 14** App Router · TypeScript
- **CSS custom properties** (système de thème light/dark)
- **Space Grotesk** (Google Fonts)
- **Azure Functions .NET 8** → backend webhook (à créer séparément)

## Démarrage rapide

```bash
# Installer les dépendances
npm install

# Configurer les variables d'environnement
cp .env.example .env.local

# Lancer en dev
npm run dev
```

→ Ouvrir http://localhost:3000

## Structure

```
app/
  page.tsx               Landing page (/)
  dashboard/page.tsx     Dashboard utilisateur (/dashboard)
  demo/page.tsx          Bot prototype interactif (/demo)
  api/
    waitlist/route.ts    POST /api/waitlist — inscription waitlist
    invoices/route.ts    GET/POST /api/invoices
    webhook/route.ts     GET/POST /api/webhook — Meta WhatsApp

components/
  landing/               Navbar, Hero, WhatsAppChat, Pricing, Waitlist...
  dashboard/             Sidebar, StatsGrid, InvoiceTable, LogoUpload...
  demo/                  PhoneMockup, ScenarioSelector, TechnicalPipeline
  ui/                    ThemeToggle, LangSwitcher

hooks/
  useTheme.ts            Dark/light toggle + localStorage
  useLang.ts             FR/EN switcher + localStorage
  useWhatsAppAnimation.ts  Animation chat séquentielle (landing)
  useBotAnimation.ts     Animation bot (page demo)

lib/
  types.ts               Types TypeScript
  data.ts                Données statiques (factures, scénarios, commandes WA)
  i18n.ts                Traductions FR/EN
```

## Variables d'environnement

Copier `.env.example` en `.env.local` et remplir :

| Variable | Description |
|---|---|
| `META_API_TOKEN` | Token Meta WhatsApp Cloud API |
| `META_PHONE_NUMBER_ID` | ID du numéro WhatsApp |
| `META_WEBHOOK_VERIFY_TOKEN` | Token de vérification webhook |
| `AZURE_OPENAI_ENDPOINT` | Endpoint Azure OpenAI |
| `AZURE_OPENAI_KEY` | Clé Azure OpenAI |
| `AZURE_COSMOS_CONNECTION_STRING` | Cosmos DB serverless |
| `AZURE_BLOB_CONNECTION_STRING` | Azure Blob Storage (PDFs + logos) |
| `STRIPE_SECRET_KEY` | Stripe payments |

## Architecture backend (à connecter)

```
WhatsApp User
    ↓ message
Meta Cloud API
    ↓ POST
/api/webhook (Next.js)
    ↓ proxy
Azure Function (.NET 8)
    ├── Azure OpenAI GPT-4o mini  → NLP extraction
    ├── QuestPDF                  → génération PDF
    ├── Azure Blob Storage        → stockage PDF
    └── Meta Cloud API            → envoi WhatsApp reply
```

## TODOs backend

- [ ] `app/api/webhook/route.ts` → connecter à Azure Function
- [ ] `app/api/waitlist/route.ts` → insérer dans Cosmos DB
- [ ] `app/api/invoices/route.ts` → CRUD Cosmos DB
- [ ] Auth (NextAuth.js ou Clerk)
- [ ] Stripe webhooks (abonnement Pro/Business)
- [ ] Upload logo → Azure Blob Storage

## Design system

Thème via CSS custom properties sur `[data-theme]` (voir `app/globals.css`).
Couleurs principales : `#25D366` (WhatsApp vert), `#080C14` (dark bg), `#F7F8F5` (light bg).
