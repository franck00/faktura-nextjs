export function Footer() {
  return (
    <footer style={{ maxWidth: 1200, margin: '0 auto', padding: '28px 32px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 12, borderTop: '1px solid var(--border)' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
        <div style={{ width: 26, height: 26, background: '#25D366', borderRadius: 6, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12 }}>📄</div>
        <span style={{ fontWeight: 600, fontSize: 14, letterSpacing: '-0.02em' }}>Faktura</span>
      </div>
      <p style={{ fontSize: 13, color: 'var(--dim)' }}>© 2025 Faktura · Facturation WhatsApp pour l'Afrique</p>
      <div style={{ display: 'flex', gap: 20 }}>
        {['Confidentialité', 'CGU', 'Contact'].map(l => (
          <a key={l} href="#" style={{ fontSize: 13, color: 'var(--dim)' }}>{l}</a>
        ))}
      </div>
    </footer>
  );
}
