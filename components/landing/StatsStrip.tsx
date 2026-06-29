export function StatsStrip() {
  const stats = [
    { value: '200M', label: 'Utilisateurs WA Business', accent: false },
    { value: '$5',   label: 'par mois pour commencer', accent: true },
    { value: '<15s', label: 'pour générer une facture', accent: false },
    { value: '6',    label: 'devises africaines', accent: false },
  ];
  return (
    <div style={{ borderTop: '1px solid var(--border)', borderBottom: '1px solid var(--border)', padding: '40px 0', background: 'var(--card)', boxShadow: 'var(--shadow)' }}>
      <div style={{ maxWidth: 1200, margin: '0 auto', display: 'grid', gridTemplateColumns: 'repeat(4,1fr)', padding: '0 32px' }}>
        {stats.map((s, i) => (
          <div key={s.value} style={{ textAlign: 'center', padding: '0 20px', borderRight: i < 3 ? '1px solid var(--border)' : undefined }}>
            <div style={{ fontSize: 38, fontWeight: 700, letterSpacing: '-0.04em', lineHeight: 1, color: s.accent ? '#25D366' : 'var(--text)' }}>{s.value}</div>
            <div style={{ fontSize: 13, color: 'var(--muted)', marginTop: 6, lineHeight: 1.4 }}>{s.label}</div>
          </div>
        ))}
      </div>
    </div>
  );
}
