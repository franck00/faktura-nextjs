'use client';
import { useWhatsAppAnimation } from '@/hooks/useWhatsAppAnimation';

export function WhatsAppChat() {
  const step = useWhatsAppAnimation();

  const UserBubble = ({ text }: { text: string }) => (
    <div style={{ display: 'flex', justifyContent: 'flex-end' }} className="fade-up">
      <div style={{
        background: '#25D366', color: '#000', borderRadius: '12px 2px 12px 12px',
        padding: '10px 14px', maxWidth: '80%', fontSize: 13, lineHeight: 1.55, fontWeight: 500,
      }}>
        {text}
        <div style={{ textAlign: 'right', fontSize: 10, marginTop: 5, opacity: 0.5 }}>08:14 ✓✓</div>
      </div>
    </div>
  );

  const TypingDot = ({ delay }: { delay: number }) => (
    <span style={{
      width: 7, height: 7, borderRadius: '50%', background: '#25D366',
      display: 'inline-block',
      animation: `dotBounce 1s ease-in-out ${delay}s infinite`,
    }} />
  );

  return (
    <div style={{
      background: 'var(--chat-bot)', borderRadius: 24, overflow: 'hidden',
      border: '1px solid var(--border)', boxShadow: 'var(--shadow-lg)', width: 340,
    }}>
      {/* WA Header */}
      <div style={{ background: '#075E54', padding: '14px 18px', display: 'flex', alignItems: 'center', gap: 12 }}>
        <div style={{ width: 40, height: 40, borderRadius: '50%', background: '#25D366', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 18 }}>🤖</div>
        <div>
          <div style={{ fontWeight: 600, fontSize: 15, color: 'white' }}>Faktura Bot</div>
          <div style={{ fontSize: 12, color: 'rgba(255,255,255,0.6)' }}>● En ligne</div>
        </div>
      </div>

      {/* Chat */}
      <div style={{ padding: 16, minHeight: 300, display: 'flex', flexDirection: 'column', gap: 10, background: 'var(--chat-bg)' }}>
        <div style={{ textAlign: 'center', marginBottom: 4 }}>
          <span style={{ fontSize: 11, color: 'var(--muted)', background: 'rgba(0,0,0,0.06)', padding: '3px 12px', borderRadius: 6 }}>Aujourd'hui</span>
        </div>

        {step >= 1 && <UserBubble text="Facture 500 000 XAF pour Groupe Orange CI, consulting mars 2025" />}

        {step === 2 && (
          <div style={{ display: 'flex', alignItems: 'center', gap: 8 }} className="fade-up">
            <div style={{ width: 30, height: 30, borderRadius: '50%', background: '#128C7E', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14 }}>🤖</div>
            <div style={{ background: 'var(--chat-bot)', borderRadius: '2px 12px 12px 12px', padding: '12px 16px', display: 'flex', gap: 5, alignItems: 'center', boxShadow: 'var(--shadow)' }}>
              <TypingDot delay={0} /><TypingDot delay={0.18} /><TypingDot delay={0.36} />
            </div>
          </div>
        )}

        {step >= 3 && (
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 8 }} className="fade-up">
            <div style={{ width: 30, height: 30, borderRadius: '50%', background: '#128C7E', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14, flexShrink: 0 }}>🤖</div>
            <div style={{ background: 'var(--chat-bot)', borderRadius: '2px 12px 12px 12px', padding: '10px 14px', fontSize: 13, color: 'var(--text)', lineHeight: 1.55, boxShadow: 'var(--shadow)' }}>
              ✅ Facture <strong>#F-2025-001</strong> créée !
              <div style={{ fontSize: 10, color: 'var(--muted)', marginTop: 4 }}>08:14</div>
            </div>
          </div>
        )}

        {step >= 4 && (
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 8 }} className="fade-up">
            <div style={{ width: 30, height: 30, borderRadius: '50%', background: '#128C7E', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14, flexShrink: 0 }}>🤖</div>
            <div style={{ background: 'var(--chat-bot)', borderRadius: '2px 12px 12px 12px', padding: '12px 14px', minWidth: 210, boxShadow: 'var(--shadow)' }}>
              <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
                <div style={{ width: 38, height: 48, background: '#DC2626', borderRadius: 6, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 10, fontWeight: 700, color: 'white' }}>PDF</div>
                <div>
                  <div style={{ fontSize: 13, fontWeight: 600, color: 'var(--text)' }}>Facture-F2025-001.pdf</div>
                  <div style={{ fontSize: 11, color: 'var(--muted)', marginTop: 3 }}>48 KB · Appuyer pour ouvrir</div>
                </div>
              </div>
              <div style={{ fontSize: 10, color: 'var(--muted)', marginTop: 8, textAlign: 'right' }}>08:14</div>
            </div>
          </div>
        )}

        {step >= 5 && (
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 8 }} className="fade-up">
            <div style={{ width: 30, height: 30, borderRadius: '50%', background: '#128C7E', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14, flexShrink: 0 }}>🤖</div>
            <div style={{ background: 'var(--chat-bot)', borderRadius: '2px 12px 12px 12px', padding: '10px 14px', fontSize: 13, color: 'var(--text)', lineHeight: 1.55, boxShadow: 'var(--shadow)' }}>
              Envoyée en <strong style={{ color: 'var(--green-text)' }}>11 secondes</strong> ⚡
              <div style={{ fontSize: 10, color: 'var(--muted)', marginTop: 4 }}>08:14</div>
            </div>
          </div>
        )}
      </div>

      {/* Input bar */}
      <div style={{ background: 'var(--card)', padding: '10px 14px', display: 'flex', alignItems: 'center', gap: 8, borderTop: '1px solid var(--border)' }}>
        <div style={{ flex: 1, background: 'var(--bg2)', borderRadius: 22, padding: '10px 16px', fontSize: 13, color: 'var(--dim)' }}>Envoie une facture...</div>
        <div style={{ width: 38, height: 38, borderRadius: '50%', background: '#25D366', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 16, cursor: 'pointer' }}>↑</div>
      </div>
    </div>
  );
}
