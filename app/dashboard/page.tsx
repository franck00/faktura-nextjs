'use client';
import { useState }            from 'react';
import { Sidebar }             from '@/components/dashboard/Sidebar';
import { StatsGrid }           from '@/components/dashboard/StatsGrid';
import { InvoiceTable }        from '@/components/dashboard/InvoiceTable';
import { LogoUpload }          from '@/components/dashboard/LogoUpload';
import { WA_COMMANDS }         from '@/lib/data';

function RevenueChart() {
  const bars = [
    { label: 'Oct', h: '8%',  color: 'rgba(37,211,102,0.18)' },
    { label: 'Nov', h: '20%', color: 'rgba(37,211,102,0.22)' },
    { label: 'Déc', h: '42%', color: 'rgba(37,211,102,0.28)' },
    { label: 'Jan', h: '55%', color: 'rgba(37,211,102,0.35)' },
    { label: 'Fév', h: '72%', color: 'rgba(37,211,102,0.5)'  },
    { label: 'Mar', h: '100%',color: '#25D366'               },
  ];
  return (
    <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, padding: 24 }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 20 }}>
        <div style={{ fontSize: 15, fontWeight: 600 }}>Revenus mensuels (XAF)</div>
        <div style={{ fontSize: 12, color: 'var(--muted)', background: 'var(--bg)', padding: '4px 12px', borderRadius: 6 }}>Oct 2024 — Mars 2025</div>
      </div>
      <div style={{ display: 'flex', alignItems: 'flex-end', gap: 16, height: 72 }}>
        {bars.map(b => (
          <div key={b.label} style={{ flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4, height: '100%', justifyContent: 'flex-end' }}>
            <div style={{ width: '100%', background: b.color, borderRadius: '4px 4px 0 0', height: b.h, transition: 'height 0.3s' }} />
          </div>
        ))}
      </div>
      <div style={{ display: 'flex', gap: 28, marginTop: 16 }}>
        <div><div style={{ fontSize: 11, color: 'var(--dim)' }}>Ce mois</div><div style={{ fontSize: 16, fontWeight: 700, color: '#25D366' }}>3 450 000 XAF</div></div>
        <div><div style={{ fontSize: 11, color: 'var(--dim)' }}>Précédent</div><div style={{ fontSize: 16, fontWeight: 700 }}>2 180 000 XAF</div></div>
        <div><div style={{ fontSize: 11, color: 'var(--dim)' }}>Croissance</div><div style={{ fontSize: 16, fontWeight: 700, color: '#25D366' }}>+58%</div></div>
      </div>
    </div>
  );
}

function WACommands() {
  return (
    <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, padding: 24 }}>
      <div style={{ fontSize: 15, fontWeight: 600, marginBottom: 3 }}>Commandes WhatsApp</div>
      <div style={{ fontSize: 13, color: 'var(--muted)', marginBottom: 16 }}>Tape directement dans la conversation bot</div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        {WA_COMMANDS.map(c => (
          <div key={c.cmd} style={{ display: 'flex', alignItems: 'center', gap: 10, padding: '9px 12px', background: 'var(--bg)', borderRadius: 9 }}>
            <code style={{ fontSize: 12, color: '#25D366', background: 'rgba(37,211,102,0.1)', padding: '3px 9px', borderRadius: 5, fontFamily: 'monospace', flexShrink: 0 }}>{c.cmd}</code>
            <span style={{ fontSize: 13, color: 'var(--muted)' }}>{c.desc}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

export default function DashboardPage() {
  const [page, setPage] = useState('dashboard');
  return (
    <div style={{ display: 'flex', height: '100vh', background: 'var(--bg)', overflow: 'hidden' }}>
      <Sidebar activePage={page} onNavigate={setPage} />
      <main style={{ flex: 1, overflowY: 'auto', minWidth: 0 }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '28px 36px 0', marginBottom: 28 }}>
          <div>
            <h1 style={{ fontSize: 22, fontWeight: 700, letterSpacing: '-0.03em', marginBottom: 3 }}>Bonjour, Kouamé 👋</h1>
            <p style={{ fontSize: 13, color: 'var(--muted)' }}>Lundi 31 mars 2025 · Plan Pro · 6 factures ce mois</p>
          </div>
          <button style={{ display: 'flex', alignItems: 'center', gap: 8, background: '#25D366', color: '#000', fontWeight: 700, fontSize: 14, padding: '11px 20px', borderRadius: 10, border: 'none', cursor: 'pointer', fontFamily: 'Space Grotesk, sans-serif' }}>
            <span style={{ fontSize: 16 }}>+</span> Nouvelle facture
          </button>
        </div>

        <div style={{ padding: '0 36px 36px' }}>
          {page === 'dashboard' && (
            <>
              <StatsGrid />
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 300px', gap: 14, marginBottom: 20 }}>
                <RevenueChart />
                {/* Top clients */}
                <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, padding: 22 }}>
                  <div style={{ fontSize: 15, fontWeight: 600, marginBottom: 16 }}>Top clients</div>
                  {[
                    { initials:'BO', name:"Bolloré Africa", amount:'1.2M XAF', invoices:'3', color:'#25D366' },
                    { initials:'OC', name:"Orange CI",       amount:'750K XAF', invoices:'2', color:'#F59E0B' },
                    { initials:'AI', name:"Air CI",          amount:'750K XAF', invoices:'1', color:'#3B82F6' },
                    { initials:'EB', name:"Ecobank",         amount:'450K XAF', invoices:'1', color:'#8B5CF6' },
                  ].map(c => (
                    <div key={c.name} style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 12 }}>
                      <div style={{ width: 34, height: 34, borderRadius: 9, background: `${c.color}1A`, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 11, fontWeight: 700, color: c.color, flexShrink: 0 }}>{c.initials}</div>
                      <div style={{ flex: 1, minWidth: 0 }}>
                        <div style={{ fontSize: 13, fontWeight: 500, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{c.name}</div>
                        <div style={{ fontSize: 11, color: 'var(--muted)' }}>{c.invoices} facture{parseInt(c.invoices)>1?'s':''}</div>
                      </div>
                      <div style={{ fontSize: 13, fontWeight: 600, whiteSpace: 'nowrap', flexShrink: 0 }}>{c.amount}</div>
                    </div>
                  ))}
                </div>
              </div>
            </>
          )}

          <InvoiceTable />

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
            <LogoUpload />
            <WACommands />
          </div>
        </div>
      </main>
    </div>
  );
}
