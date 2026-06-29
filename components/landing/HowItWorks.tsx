'use client';
import { useLang } from '@/hooks/useLang';

export function HowItWorks() {
  const { t } = useLang();
  const steps = [
    { n: '01', icon: '💬', title: t.s1title, body: t.s1body, featured: false },
    { n: '02', icon: '🤖', title: t.s2title, body: t.s2body, featured: true, tag: t.s2tag },
    { n: '03', icon: '📎', title: t.s3title, body: t.s3body, featured: false },
  ];
  return (
    <section id="how" style={{ maxWidth: 1200, margin: '0 auto', padding: '96px 32px' }}>
      <div style={{ textAlign: 'center', marginBottom: 64 }}>
        <p style={{ fontSize: 12, color: 'var(--green-text)', fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.12em', marginBottom: 14 }}>{t.howLabel}</p>
        <h2 style={{ fontSize: 'clamp(28px,4vw,44px)', fontWeight: 700, letterSpacing: '-0.03em' }}>{t.howTitle}</h2>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: 24 }}>
        {steps.map(s => (
          <div key={s.n} style={{
            background: 'var(--card)', border: s.featured ? '2px solid #25D366' : '1px solid var(--border)',
            borderRadius: 20, padding: 32, position: 'relative', overflow: 'hidden',
            boxShadow: s.featured ? '0 8px 32px rgba(37,211,102,0.12)' : 'var(--shadow)',
          }}>
            <div style={{ position: 'absolute', top: 18, right: 22, fontSize: 48, fontWeight: 800, color: 'rgba(0,0,0,0.03)', fontFamily: 'monospace', lineHeight: 1, userSelect: 'none' }}>{s.n}</div>
            <div style={{ width: 48, height: 48, background: 'var(--green-bg)', border: '1px solid var(--green-bd)', borderRadius: 14, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 22, marginBottom: 20 }}>{s.icon}</div>
            <h3 style={{ fontSize: 18, fontWeight: 600, marginBottom: 10, letterSpacing: '-0.02em' }}>{s.title}</h3>
            <p style={{ fontSize: 14, color: 'var(--muted)', lineHeight: 1.7 }}>{s.body}</p>
            {s.tag && <div style={{ marginTop: 16, display: 'inline-block', background: 'var(--green-bg)', borderRadius: 6, padding: '4px 10px', fontSize: 12, color: 'var(--green-text)', fontWeight: 600 }}>{s.tag}</div>}
          </div>
        ))}
      </div>
    </section>
  );
}
