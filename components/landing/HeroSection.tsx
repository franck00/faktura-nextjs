'use client';
import { WhatsAppChat } from './WhatsAppChat';
import { useLang } from '@/hooks/useLang';

export function HeroSection() {
  const { t } = useLang();
  return (
    <section style={{ maxWidth: 1200, margin: '0 auto', padding: '84px 32px 60px', display: 'flex', gap: 72, alignItems: 'center', flexWrap: 'wrap' }}>
      <div style={{ flex: 1, minWidth: 300 }}>
        <div style={{ display: 'inline-flex', alignItems: 'center', gap: 8, background: 'var(--green-bg)', border: '1px solid var(--green-bd)', borderRadius: 100, padding: '6px 14px', marginBottom: 24 }}>
          <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#25D366', display: 'inline-block', animation: 'pulse 2s ease infinite' }} />
          <span style={{ fontSize: 13, color: 'var(--green-text)', fontWeight: 600 }}>{t.waitlistLabel}</span>
        </div>
        <h1 style={{ fontSize: 'clamp(36px,5vw,64px)', fontWeight: 700, lineHeight: 1.06, letterSpacing: '-0.035em', marginBottom: 22 }}>
          {t.h1a}<br />
          <span style={{ color: '#25D366' }}>{t.h1b}</span><br />
          <span style={{ fontSize: '0.55em', fontWeight: 500, color: 'var(--muted)', letterSpacing: '-0.01em' }}>{t.h1c}</span>
        </h1>
        <p style={{ fontSize: 18, color: 'var(--muted)', maxWidth: 440, lineHeight: 1.7, marginBottom: 36, fontWeight: 400 }}>{t.sub}</p>
        <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', marginBottom: 28 }}>
          <a href="#waitlist" style={{ background: '#25D366', color: '#000', fontWeight: 700, fontSize: 16, padding: '15px 32px', borderRadius: 12, display: 'inline-block', letterSpacing: '-0.01em' }}>{t.ctaPrimary}</a>
          <a href="#how" style={{ border: '1px solid var(--border)', color: 'var(--muted)', fontWeight: 500, fontSize: 16, padding: '15px 24px', borderRadius: 12, display: 'inline-block', background: 'var(--card)', boxShadow: 'var(--shadow)' }}>{t.ctaSecond}</a>
        </div>
        <p style={{ fontSize: 13, color: 'var(--dim)', display: 'flex', alignItems: 'center', gap: 6, flexWrap: 'wrap' }}>
          <span>⭐⭐⭐⭐⭐</span><span>{t.social}</span>
        </p>
      </div>
      <div style={{ flex: '0 0 340px' }}>
        <WhatsAppChat />
        <p style={{ textAlign: 'center', fontSize: 12, color: 'var(--dim)', marginTop: 14 }}>XAF · USD · EUR · GHS · NGN · MAD</p>
      </div>
    </section>
  );
}
