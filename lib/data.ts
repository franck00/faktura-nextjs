import type { Invoice, Scenario } from './types';

export const INVOICES: Invoice[] = [
  { id: 'F-2025-006', client: 'Ecobank Abidjan',         amount: '450 000',   currency: 'XAF', status: 'paid',    date: '28 mars', desc: 'Conseil transformation digitale' },
  { id: 'F-2025-005', client: 'Total Energies CI',        amount: '300 000',   currency: 'XAF', status: 'overdue', date: '05 mars', desc: 'Support IT mensuel' },
  { id: 'F-2025-004', client: "Air Côte d'Ivoire",        amount: '750 000',   currency: 'XAF', status: 'pending', date: '22 mars', desc: 'Développement web mars' },
  { id: 'F-2025-003', client: 'Bolloré Africa Logistics', amount: '1 200 000', currency: 'XAF', status: 'paid',    date: '20 mars', desc: 'Audit logistique Q1' },
  { id: 'F-2025-002', client: 'MTN Cameroun',             amount: '250 000',   currency: 'XAF', status: 'pending', date: '18 mars', desc: 'Formation IA équipe' },
  { id: 'F-2025-001', client: 'Groupe Orange CI',         amount: '500 000',   currency: 'XAF', status: 'paid',    date: '15 mars', desc: 'Consulting mars 2025' },
];

export const SCENARIOS: Record<string, Scenario> = {
  invoice: {
    delays: [500, 1300, 2700, 3700, 4600],
    msgs: [
      { kind: 'user', text: 'Facture 500 000 XAF pour Groupe Orange CI, consulting mars 2025' },
      { kind: 'typing' },
      { kind: 'bot',  text: '✅ Compris !\n📌 Client : Groupe Orange CI\n💰 500 000 XAF\n📋 Consulting mars 2025\n\nGénération en cours...' },
      { kind: 'pdf',  pdfName: 'Facture-F2025-001.pdf', pdfSize: '48 KB' },
      { kind: 'bot',  text: '✅ Facture #F-2025-001 envoyée !\nTraitée en 11 secondes ⚡' },
    ],
  },
  multi: {
    delays: [500, 1300, 2700, 3700, 4400],
    msgs: [
      { kind: 'user', text: 'Invoice 2500 EUR for TotalEnergies Paris, IT consulting April 2025' },
      { kind: 'typing' },
      { kind: 'bot',  text: '✅ Got it!\n📌 Client: TotalEnergies Paris\n💰 2,500 EUR (≈ 1 650 000 XAF)\n\nGenerating...' },
      { kind: 'pdf',  pdfName: 'Invoice-F2025-007.pdf', pdfSize: '52 KB' },
      { kind: 'bot',  text: '✅ Invoice #F-2025-007 sent!\nProcessed in 9 seconds ⚡' },
    ],
  },
  devis: {
    delays: [400, 1300, 3400, 4400, 5800, 6700],
    msgs: [
      { kind: 'user', text: '/devis' },
      { kind: 'bot',  text: 'Décris le devis 👇\n\n"Devis [montant] [devise] pour [client], [objet]"' },
      { kind: 'user', text: 'Devis 800 000 XAF pour MTN Cameroun, développement app mobile Q2 2025' },
      { kind: 'typing' },
      { kind: 'pdf',  pdfName: 'Devis-D2025-004.pdf', pdfSize: '44 KB' },
      { kind: 'bot',  text: '✅ Devis #D-2025-004 prêt !\nValide 30 jours.\n\nTape /confirmer pour convertir en facture.' },
    ],
  },
  histo: {
    delays: [400, 1100, 2300],
    msgs: [
      { kind: 'user', text: '/historique' },
      { kind: 'typing' },
      { kind: 'bot',  text: '📋 Tes 4 dernières factures :\n\n1 · F-006 · Ecobank · 450K XAF ✅\n2 · F-005 · Total CI · 300K XAF ⏳\n3 · F-004 · Air CI · 750K XAF ⏳\n4 · F-003 · Bolloré · 1.2M XAF ✅\n\n💰 En attente : 1 050 000 XAF' },
    ],
  },
};

export const WA_COMMANDS = [
  { cmd: '/facture',    desc: 'Créer une nouvelle facture' },
  { cmd: '/devis',      desc: 'Générer un devis PDF' },
  { cmd: '/clients',    desc: 'Voir ta liste clients' },
  { cmd: '/historique', desc: '10 dernières factures' },
  { cmd: '/logo',       desc: 'Envoyer ton logo' },
];
