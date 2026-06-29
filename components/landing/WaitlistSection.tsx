'use client';
import { useState } from 'react';
import { useLang } from '@/hooks/useLang';

export function WaitlistSection() {
  const { t } = useLang();
  const [email, setEmail] = useState('');
  const [sent,  setSent]  = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!email) return;
    try {
      await fetch('/api/waitlist', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email }),
      });
    } catch {}
    setSent(true);
  }

  return (
    <section id="waitlist" style={{ background: 'var(--card)', borderTop: '1px solid var(--border)', padding: '96px 32px', boxShadow: 'var(--shadow)' }}>
      <div style={{ maxWidth: 580, margin: '0 auto', textAlign: 'center' }}>
        <p style={{ fontSize: 12, color: 'var(--green-text)', fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.12em', marginBottom: 16 }}>{t.waitlistLabel}</p>
        <h2 style={{ fontSize: 'clamp(28px,4vw,44px)', fontWeight: 700, letterSpacing: '-0.03em', marginBottom: 16, lineHeight: 1.1 }}>{t.waitlistTitle}</h2>
        <p style={{ fontSize: 16, color: 'var(--muted)', marginBottom: 40, lineHeight: 1.7 }}>{t.waitlistSub}</p>

        {sent ? (
          <div className="fade-up" style={{ background: 'var(--green-bg)', border: '1px solid var(--green-bd)', borderRadius: 16, padding: 36 }}>
            <div style={{ fontSize: 44, marginBottom: 14 }}>🎉</div>
            <h3 style={{ fontSize: 22, fontWeight: 700, color: 'var(--green-text)', marginBottom: 10, letterSpacing: '-0.02em' }}>{t.waitlistConf}</h3>
            <p style={{ fontSize: 15, color: 'var(--muted)', lineHeight: 1.65 }}>{t.waitlistConfSub}</p>
          </div>
        ) : (
          <>
            <form onSubmit={handleSubmit} style={{ display: 'flex', gap: 10, flexWrap: 'wrap', justifyContent: 'center' }}>
              <input
                type="email" required value={email}
                onChange={e => setEmail(e.target.value)}
                placeholder="ton@email.com"
                style={{
                  flex: 1, minWidth: 220, background: 'var(--bg)',
                  border: '1px solid var(--border)', borderRadius: 10,
                  padding: '14px 18px', fontSize: 15, color: 'var(--text)',
                  fontFamily: 'Space Grotesk, sans-serif', outline: 'none',
                  transition: 'border-color 0.2s, box-shadow 0.2s',
                }}
                onFocus={e => { e.target.style.borderColor = '#25D366'; e.target.style.boxShadow = '0 0 0 3px rgba(37,211,102,0.12)'; }}
                onBlur={e =>  { e.target.style.borderColor = 'var(--border)'; e.target.style.boxShadow = 'none'; }}
              />
              <button type="submit" style={{ background: '#25D366', color: '#000', fontWeight: 700, fontSize: 15, padding: '14px 28px', borderRadius: 10, border: 'none', cursor: 'pointer', fontFamily: 'Space Grotesk, sans-serif', whiteSpace: 'nowrap' }}>
                {t.submitBtn}
              </button>
            </form>
            <p style={{ fontSize: 12, color: 'var(--dim)', marginTop: 14 }}>{t.noCard}</p>
          </>
        )}
      </div>
    </section>
  );
}
