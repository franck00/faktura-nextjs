'use client';
import { useLang } from '@/hooks/useLang';

export function PricingSection() {
  const { t } = useLang();
  const isEn = false; // use useLang().lang === 'en' if needed

  const Feature = ({ text, included }: { text: string; included: boolean }) => (
    <div style={{ display: 'flex', gap: 10, alignItems: 'center', fontSize: 14, color: included ? 'var(--text2)' : 'var(--dim)' }}>
      <span style={{ color: included ? '#25D366' : 'var(--dim)', fontSize: 16, flexShrink: 0 }}>{included ? '✓' : '✗'}</span>
      {text}
    </div>
  );

  return (
    <section id="pricing" style={{ maxWidth: 1200, margin: '0 auto', padding: '0 32px 96px' }}>
      <div style={{ textAlign: 'center', marginBottom: 64 }}>
        <p style={{ fontSize: 12, color: 'var(--green-text)', fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.12em', marginBottom: 14 }}>Tarifs</p>
        <h2 style={{ fontSize: 'clamp(28px,4vw,44px)', fontWeight: 700, letterSpacing: '-0.03em' }}>Simple. Transparent. Africain.</h2>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: 20, alignItems: 'start' }}>
        {/* Starter */}
        <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 20, padding: 32, boxShadow: 'var(--shadow)' }}>
          <p style={{ fontSize: 13, fontWeight: 600, color: 'var(--muted)', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: 14 }}>Starter</p>
          <div style={{ fontSize: 46, fontWeight: 700, letterSpacing: '-0.04em', lineHeight: 1, marginBottom: 5 }}>$5<span style={{ fontSize: 16, fontWeight: 400, color: 'var(--muted)' }}>/mois</span></div>
          <p style={{ fontSize: 13, color: 'var(--dim)', marginBottom: 28 }}>≈ 3 000 XAF · Indépendants</p>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 11, marginBottom: 28, paddingBottom: 28, borderBottom: '1px solid var(--border)' }}>
            <Feature text="30 factures / mois" included /><Feature text="1 devise (XAF ou USD)" included /><Feature text="Templates de base" included />
            <Feature text="Logo personnalisé" included={false} /><Feature text="Dashboard web" included={false} />
          </div>
          <a href="#waitlist" style={{ display: 'block', textAlign: 'center', border: '1px solid var(--border)', color: 'var(--text2)', fontWeight: 600, fontSize: 14, padding: 13, borderRadius: 10, background: 'var(--bg2)' }}>Essai gratuit 7 jours</a>
        </div>

        {/* Pro */}
        <div style={{ background: '#25D366', borderRadius: 20, padding: 32, position: 'relative', marginTop: -10, boxShadow: '0 24px 48px rgba(37,211,102,0.22)' }}>
          <div style={{ position: 'absolute', top: -15, left: '50%', transform: 'translateX(-50%)', background: '#111', color: '#fff', fontSize: 12, fontWeight: 700, padding: '4px 16px', borderRadius: 100, whiteSpace: 'nowrap' }}>⭐ Le plus populaire</div>
          <p style={{ fontSize: 13, fontWeight: 700, color: 'rgba(0,0,0,0.55)', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: 14 }}>Pro</p>
          <div style={{ fontSize: 46, fontWeight: 700, letterSpacing: '-0.04em', lineHeight: 1, marginBottom: 5, color: '#000' }}>$15<span style={{ fontSize: 16, fontWeight: 400, color: 'rgba(0,0,0,0.5)' }}>/mois</span></div>
          <p style={{ fontSize: 13, color: 'rgba(0,0,0,0.5)', marginBottom: 28 }}>≈ 9 000 XAF · PME actives</p>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 11, marginBottom: 28, paddingBottom: 28, borderBottom: '1px solid rgba(0,0,0,0.1)' }}>
            {['Factures illimitées','6 devises (XAF, USD, EUR, GHS, NGN, MAD)','Logo + templates pro','Dashboard historique + stats','Support prioritaire'].map(f => (
              <div key={f} style={{ display: 'flex', gap: 10, alignItems: 'center', fontSize: 14, color: '#000', fontWeight: 500 }}><span style={{ fontSize: 16 }}>✓</span>{f}</div>
            ))}
          </div>
          <a href="#waitlist" style={{ display: 'block', textAlign: 'center', background: '#000', color: '#fff', fontWeight: 700, fontSize: 15, padding: 15, borderRadius: 10 }}>Commencer — 7 jours gratuits</a>
        </div>

        {/* Business */}
        <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 20, padding: 32, boxShadow: 'var(--shadow)' }}>
          <p style={{ fontSize: 13, fontWeight: 600, color: 'var(--muted)', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: 14 }}>Business</p>
          <div style={{ fontSize: 46, fontWeight: 700, letterSpacing: '-0.04em', lineHeight: 1, marginBottom: 5 }}>$39<span style={{ fontSize: 16, fontWeight: 400, color: 'var(--muted)' }}>/mois</span></div>
          <p style={{ fontSize: 13, color: 'var(--dim)', marginBottom: 28 }}>Équipes et agences</p>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 11, marginBottom: 28, paddingBottom: 28, borderBottom: '1px solid var(--border)' }}>
            {['Tout ce qui est Pro','CRM clients intégré','Analytics avancés','Accès API complet','Facturation équipe'].map(f => (
              <Feature key={f} text={f} included />
            ))}
          </div>
          <a href="#waitlist" style={{ display: 'block', textAlign: 'center', border: '1px solid var(--border)', color: 'var(--text2)', fontWeight: 600, fontSize: 14, padding: 13, borderRadius: 10, background: 'var(--bg2)' }}>Contacter l'équipe</a>
        </div>
      </div>
      <p style={{ textAlign: 'center', marginTop: 20, fontSize: 13, color: 'var(--dim)' }}>Aucune carte requise · Annulation à tout moment · Mobile Money disponible bientôt</p>
    </section>
  );
}
