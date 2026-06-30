# PieceBot

> SaaS B2B qui permet aux cabinets comptables d'Afrique francophone d'**automatiser la collecte mensuelle des pièces justificatives** de leurs clients via WhatsApp, et de les classer automatiquement par client dans un dashboard web.

Flux nominal : photo facture par WhatsApp → webhook → OCR → extraction structurée (montant, date, fournisseur, TVA) → base de données → accusé WhatsApp → dashboard cabinet.

---

## Architecture (monorepo)

```
piecebot/
├── apps/
│   ├── web/                     # Frontend Next.js 14 (App Router, TS, Tailwind)
│   │   ├── app/  components/  hooks/  lib/
│   │   └── package.json
│   │
│   └── api/                     # Backend .NET 10 (Web API, Controllers)
│       ├── PieceBot.slnx
│       ├── src/
│       │   ├── PieceBot.Api/            # Couche HTTP : Controllers, Program.cs, DI
│       │   ├── PieceBot.Core/           # Domaine : entités, enums, interfaces (Abstractions)
│       │   └── PieceBot.Infrastructure/ # Accès données : repositories (Cosmos à venir)
│       └── tests/
│           └── PieceBot.Tests/          # Tests xUnit
│
├── .gitignore
└── README.md
```

**Séparation des responsabilités (backend) :**

- `PieceBot.Core` — le **domaine** pur (entités `EndClient`, `Piece`, enums, `ExtractedData`) et les **interfaces** de repository. Ne dépend de rien d'externe.
- `PieceBot.Infrastructure` — les **implémentations** (aujourd'hui en mémoire, demain Cosmos DB / Blob Storage / Document Intelligence). Branchée via `AddInfrastructure()`.
- `PieceBot.Api` — la couche **HTTP** : Controllers minces qui délèguent à `Core` via les interfaces. CORS ouvert pour `apps/web`.

Le front et le back sont **indépendants** (build et CI séparés).

> ⚠️ Transition en cours : certaines routes API TypeScript (`apps/web/app/api/*`) sont **provisoires** et seront portées vers le backend .NET, puis retirées au profit d'appels HTTP vers `apps/api`.

---

## Démarrer en local

### Frontend (`apps/web`)

```bash
cd apps/web
npm install
cp .env.example .env.local   # remplir les secrets
npm run dev                  # http://localhost:3000
```

### Backend (`apps/api`)

Prérequis : **.NET 10 SDK**.

```bash
cd apps/api
dotnet build PieceBot.slnx
dotnet test  PieceBot.slnx
dotnet run --project src/PieceBot.Api   # https://localhost:7xxx
```

Endpoints disponibles (squelette) :

| Méthode | Route | Description |
|---|---|---|
| GET | `/health` | Sonde de santé |
| GET | `/api/endclients?search=` | Liste des clients du cabinet |
| GET | `/api/endclients/{id}` | Détail d'un client |

---

## Stack cible (spec)

- **Backend** : .NET 10 Web API + Azure Functions, Cosmos DB (partition `tenantId`), Blob Storage, Document Intelligence (OCR), Azure OpenAI, Key Vault, App Insights.
- **Frontend** : Next.js 14 + TypeScript + Tailwind + shadcn/ui + TanStack Query + Zod (Vercel).
- **Auth** : Clerk · **Paiement** : Stripe · **Emails** : Resend · **WhatsApp** : Meta Cloud API.

## Conventions

- **Gitflow** : `main` (prod) ← `develop` (intégration) ← `feature/…` `fix/…` `chore/…`. PR vers `develop` ; `main` ne reçoit que des releases.
- Aucun `.env` versionné (secrets via Key Vault / CI). Tous les `*.md` sont ignorés sauf ce `README.md`.
