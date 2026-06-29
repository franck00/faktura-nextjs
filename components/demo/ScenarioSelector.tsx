'use client';
import type { ScenarioKey } from '@/lib/types';

interface ScenarioSelectorProps {
  current: ScenarioKey | null;
  onSelect: (k: ScenarioKey) => void;
}

const SCENARIOS_META: { key: ScenarioKey; icon: string; title: string; desc: string }[] = [
  { key: 'invoice', icon: '🧾', title: 'Facture simple',  desc: '500 000 XAF · Orange CI' },
  { key: 'multi',   icon: '🌍', title: 'Multi-devises',   desc: '2 500 EUR · TotalEnergies' },
  { key: 'devis',   icon: '📝', title: 'Devis → Facture', desc: '800 000 XAF · MTN' },
  { key: 'histo',   icon: '📋', title: 'Historique',      desc: '/historique command' },
];

export function ScenarioSelector({ current, onSelect }: ScenarioSelectorProps) {
  return (
    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10 }}>
      {SCENARIOS_META.map(s => (
        <button key={s.key} onClick={() => onSelect(s.key)} style={{
          display: 'flex', alignItems: 'center', gap: 10, padding: '14px 16px',
          borderRadius: 12, border: `1px solid ${current === s.key ? 'rgba(37,211,102,0.25)' : 'rgba(255,255,255,0.07)'}`,
          background: current === s.key ? 'rgba(37,211,102,0.06)' : 'transparent',
          cursor: 'pointer', fontFamily: 'Space Grotesk, sans-serif', textAlign: 'left',
        }}>
          <span style={{ fontSize: 20, flexShrink: 0 }}>{s.icon}</span>
          <div>
            <div style={{ fontSize: 14, fontWeight: 600, color: 'var(--text)' }}>{s.title}</div>
            <div style={{ fontSize: 12, color: 'var(--muted)', marginTop: 1 }}>{s.desc}</div>
          </div>
        </button>
      ))}
    </div>
  );
}
