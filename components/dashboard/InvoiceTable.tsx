'use client';
import { useState } from 'react';
import { INVOICES } from '@/lib/data';
import type { InvoiceTab, Invoice } from '@/lib/types';

const STATUS_STYLE: Record<string, { bg: string; color: string; label: string }> = {
  paid:    { bg: 'rgba(37,211,102,0.1)',  color: '#25D366', label: 'Payée' },
  pending: { bg: 'rgba(251,191,36,0.1)',  color: '#F59E0B', label: 'En attente' },
  overdue: { bg: 'rgba(239,68,68,0.1)',   color: '#EF4444', label: 'En retard' },
};

export function InvoiceTable() {
  const [tab, setTab] = useState<InvoiceTab>('all');

  const filtered = INVOICES.filter(inv => {
    if (tab === 'paid')    return inv.status === 'paid';
    if (tab === 'pending') return inv.status !== 'paid';
    return true;
  });

  const tabBtn = (t: InvoiceTab, label: string) => (
    <button key={t} onClick={() => setTab(t)} style={{
      padding: '6px 14px', borderRadius: 7, border: 'none', cursor: 'pointer',
      fontFamily: 'Space Grotesk, sans-serif', fontSize: 13, fontWeight: 500,
      background: tab === t ? 'var(--card)' : 'transparent',
      color:      tab === t ? 'var(--text)' : 'var(--muted)',
    }}>{label}</button>
  );

  return (
    <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, overflow: 'hidden', marginBottom: 16 }}>
      <div style={{ padding: '18px 24px', borderBottom: '1px solid var(--border)', display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 12, flexWrap: 'wrap' }}>
        <div style={{ fontSize: 15, fontWeight: 600, letterSpacing: '-0.02em' }}>Factures récentes</div>
        <div style={{ display: 'flex', gap: 4, background: 'var(--bg)', borderRadius: 9, padding: 3 }}>
          {tabBtn('all', 'Toutes')}
          {tabBtn('pending', 'En attente')}
          {tabBtn('paid', 'Payées')}
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '130px 1fr 148px 110px 112px 92px', padding: '9px 24px', borderBottom: '1px solid var(--border)' }}>
        {['#', 'Client', 'Montant', 'Date', 'Statut', 'Actions'].map(h => (
          <div key={h} style={{ fontSize: 11, color: 'var(--dim)', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.08em', textAlign: h === 'Montant' ? 'right' : h === 'Date' || h === 'Statut' ? 'center' : h === 'Actions' ? 'right' : 'left' }}>{h}</div>
        ))}
      </div>

      {filtered.map(inv => {
        const s = STATUS_STYLE[inv.status];
        return (
          <div key={inv.id} style={{ display: 'grid', gridTemplateColumns: '130px 1fr 148px 110px 112px 92px', padding: '13px 24px', borderBottom: '1px solid var(--border)', alignItems: 'center' }}>
            <div style={{ fontSize: 12, fontWeight: 600, color: 'var(--muted)', fontFamily: 'monospace' }}>{inv.id}</div>
            <div>
              <div style={{ fontSize: 14, fontWeight: 500 }}>{inv.client}</div>
              <div style={{ fontSize: 12, color: 'var(--muted)' }}>{inv.desc}</div>
            </div>
            <div style={{ textAlign: 'right' }}>
              <div style={{ fontSize: 14, fontWeight: 600 }}>{inv.amount}</div>
              <div style={{ fontSize: 11, color: 'var(--muted)' }}>{inv.currency}</div>
            </div>
            <div style={{ textAlign: 'center', fontSize: 13, color: 'var(--muted)' }}>{inv.date}</div>
            <div style={{ textAlign: 'center' }}>
              <span style={{ display: 'inline-block', fontSize: 12, fontWeight: 500, padding: '4px 10px', borderRadius: 6, background: s.bg, color: s.color }}>{s.label}</span>
            </div>
            <div style={{ display: 'flex', gap: 5, justifyContent: 'flex-end' }}>
              {['⬇', '↗'].map(icon => (
                <button key={icon} title={icon === '⬇' ? 'Télécharger PDF' : 'Renvoyer via WhatsApp'} style={{ width: 30, height: 30, borderRadius: 7, border: '1px solid var(--border)', background: 'transparent', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 13, color: 'var(--muted)' }}>{icon}</button>
              ))}
            </div>
          </div>
        );
      })}
    </div>
  );
}
