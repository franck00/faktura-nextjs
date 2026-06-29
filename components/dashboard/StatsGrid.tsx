export function StatsGrid() {
  const stats = [
    { label: 'Total factures', value: '24',         sub: '↑ +6 ce mois',      subColor: '#25D366' },
    { label: 'CA ce mois',     value: '3 450 000',  sub: 'XAF · ≈ $5 750',    subColor: 'var(--muted)' },
    { label: 'En attente',     value: '1 050 000',  sub: 'XAF · 2 factures',   subColor: 'var(--muted)', valueColor: '#F59E0B' },
    { label: 'Clients actifs', value: '8',           sub: '↑ +2 ce mois',      subColor: '#25D366' },
  ];
  return (
    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4,1fr)', gap: 14, marginBottom: 20 }}>
      {stats.map(s => (
        <div key={s.label} style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, padding: 20 }}>
          <div style={{ fontSize: 11, color: 'var(--dim)', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.07em', marginBottom: 10 }}>{s.label}</div>
          <div style={{ fontSize: s.value.length > 5 ? 24 : 34, fontWeight: 700, letterSpacing: '-0.04em', lineHeight: 1, marginBottom: 6, color: s.valueColor ?? 'var(--text)' }}>{s.value}</div>
          <div style={{ fontSize: 12, color: s.subColor }}>{s.sub}</div>
        </div>
      ))}
    </div>
  );
}
