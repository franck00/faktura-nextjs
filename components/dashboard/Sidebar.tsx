'use client';

interface SidebarProps {
  activePage?: string;
  onNavigate?: (page: string) => void;
}

export function Sidebar({ activePage = 'dashboard', onNavigate }: SidebarProps) {
  const navItem = (page: string, icon: string, label: string, badge?: number) => {
    const active = activePage === page;
    return (
      <button key={page} onClick={() => onNavigate?.(page)} style={{
        display: 'flex', alignItems: 'center', gap: 10, padding: '9px 10px',
        borderRadius: 9, border: 'none', cursor: 'pointer',
        fontFamily: 'Space Grotesk, sans-serif', fontSize: 14, fontWeight: 500,
        textAlign: 'left', width: '100%',
        background: active ? 'rgba(37,211,102,0.08)' : 'transparent',
        color:      active ? '#25D366' : 'var(--dim)',
      }}>
        <span style={{ fontSize: 16, flexShrink: 0 }}>{icon}</span>
        {label}
        {badge !== undefined && (
          <span style={{ marginLeft: 'auto', background: 'rgba(37,211,102,0.15)', color: '#25D366', fontSize: 11, fontWeight: 700, padding: '2px 7px', borderRadius: 100 }}>{badge}</span>
        )}
      </button>
    );
  };

  return (
    <aside style={{ flex: '0 0 224px', display: 'flex', flexDirection: 'column', background: 'var(--bg)', borderRight: '1px solid var(--border)', overflow: 'hidden' }}>
      {/* Logo */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 9, padding: '20px 20px 22px' }}>
        <div style={{ width: 32, height: 32, background: '#25D366', borderRadius: 8, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 16, flexShrink: 0 }}>📄</div>
        <span style={{ fontSize: 18, fontWeight: 700, letterSpacing: '-0.03em' }}>Faktura</span>
      </div>

      {/* Nav */}
      <nav style={{ display: 'flex', flexDirection: 'column', gap: 2, padding: '0 12px', flex: 1, overflowY: 'auto' }}>
        {navItem('dashboard', '📊', 'Tableau de bord')}
        {navItem('invoices',  '🧾', 'Factures', 6)}
        {navItem('clients',   '👥', 'Clients')}
        {navItem('analytics', '📈', 'Analytics')}
        <div style={{ margin: '10px 0', height: 1, background: 'var(--border)' }} />
        {navItem('settings',  '⚙️', 'Paramètres')}
      </nav>

      {/* WhatsApp CTA */}
      <div style={{ padding: '10px 12px' }}>
        <div style={{ background: 'rgba(37,211,102,0.07)', border: '1px solid rgba(37,211,102,0.13)', borderRadius: 12, padding: 14 }}>
          <div style={{ fontSize: 18, marginBottom: 6 }}>💬</div>
          <div style={{ fontSize: 13, fontWeight: 600, marginBottom: 3 }}>Créer via WhatsApp</div>
          <div style={{ fontSize: 11, color: 'var(--muted)', marginBottom: 10, lineHeight: 1.5 }}>Envoie un message au bot pour générer une facture</div>
          <a href="https://wa.me/" target="_blank" rel="noreferrer" style={{ display: 'block', textAlign: 'center', background: '#25D366', color: '#000', fontSize: 12, fontWeight: 700, padding: 8, borderRadius: 8 }}>Ouvrir WhatsApp ↗</a>
        </div>
      </div>

      {/* User */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 10, padding: '14px 18px', borderTop: '1px solid var(--border)', marginTop: 8 }}>
        <div style={{ width: 34, height: 34, borderRadius: '50%', background: 'linear-gradient(135deg,#25D366,#128C7E)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14, fontWeight: 700, color: '#000', flexShrink: 0 }}>K</div>
        <div style={{ minWidth: 0, flex: 1 }}>
          <div style={{ fontSize: 13, fontWeight: 600, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>Kouamé Dev</div>
          <div style={{ fontSize: 11, color: '#25D366', marginTop: 1 }}>Plan Pro ✓</div>
        </div>
        <div style={{ fontSize: 18, color: 'var(--muted)', cursor: 'pointer' }}>⋮</div>
      </div>
    </aside>
  );
}
