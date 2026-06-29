'use client';
import { useEffect, useRef } from 'react';
import type { ChatMessage } from '@/lib/types';

interface PhoneMockupProps {
  messages: ChatMessage[];
  hasStarted: boolean;
}

export function PhoneMockup({ messages, hasStarted }: PhoneMockupProps) {
  const chatRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (chatRef.current) chatRef.current.scrollTop = chatRef.current.scrollHeight;
  }, [messages]);

  const TypingDot = ({ delay }: { delay: number }) => (
    <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#25D366', display: 'inline-block', animation: `dotBounce 1s ease-in-out ${delay}s infinite` }} />
  );

  return (
    <div style={{ background: '#0A0F1A', borderRadius: 44, border: '2px solid #1C2333', boxShadow: '0 40px 80px rgba(0,0,0,0.7)', overflow: 'hidden' }}>
      {/* Status bar */}
      <div style={{ height: 50, background: '#0A0F1A', display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0 22px', position: 'relative' }}>
        <div style={{ position: 'absolute', left: '50%', top: 0, transform: 'translateX(-50%)', width: 110, height: 28, background: '#080C14', borderRadius: '0 0 18px 18px' }} />
        <span style={{ fontSize: 12, fontWeight: 600, color: '#4B5563', zIndex: 1 }}>09:41</span>
        <span style={{ fontSize: 10, color: '#4B5563', zIndex: 1 }}>▊▊▊</span>
      </div>

      {/* WA Header */}
      <div style={{ background: '#075E54', padding: '10px 14px', display: 'flex', alignItems: 'center', gap: 10 }}>
        <div style={{ width: 36, height: 36, borderRadius: '50%', background: '#25D366', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 16, flexShrink: 0 }}>🤖</div>
        <div>
          <div style={{ fontWeight: 600, fontSize: 14, color: 'white', lineHeight: 1.2 }}>Faktura Bot</div>
          <div style={{ fontSize: 11, color: 'rgba(255,255,255,0.55)' }}>● En ligne</div>
        </div>
      </div>

      {/* Chat area */}
      <div ref={chatRef} style={{ height: 430, overflowY: 'auto', background: '#0A1322', padding: '12px 10px', display: 'flex', flexDirection: 'column', gap: 9 }}>
        {!hasStarted && (
          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', height: '100%', gap: 10, textAlign: 'center' }}>
            <div style={{ fontSize: 44, opacity: 0.15 }}>💬</div>
            <div style={{ fontSize: 13, color: '#2D3748', lineHeight: 1.7 }}>Sélectionne un scénario →</div>
          </div>
        )}

        {hasStarted && <div style={{ textAlign: 'center' }}><span style={{ fontSize: 10, color: '#2D3748', background: '#0F1B2A', padding: '3px 10px', borderRadius: 5 }}>Aujourd'hui</span></div>}

        {messages.map((msg, i) => {
          if (msg.kind === 'user') return (
            <div key={i} className="fade-up" style={{ display: 'flex', justifyContent: 'flex-end' }}>
              <div style={{ background: '#25D366', color: '#000', borderRadius: '10px 2px 10px 10px', padding: '9px 12px', maxWidth: '82%', fontSize: 12, lineHeight: 1.55, fontWeight: 500, whiteSpace: 'pre-line' }}>
                {msg.text}<div style={{ textAlign: 'right', fontSize: 9, marginTop: 4, opacity: 0.5 }}>09:41 ✓✓</div>
              </div>
            </div>
          );
          if (msg.kind === 'typing') return (
            <div key={i} className="fade-up" style={{ display: 'flex', alignItems: 'center', gap: 7 }}>
              <div style={{ width: 26, height: 26, borderRadius: '50%', background: '#128C7E', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12, flexShrink: 0 }}>🤖</div>
              <div style={{ background: '#1A2236', borderRadius: '2px 10px 10px 10px', padding: '11px 13px', display: 'flex', gap: 5 }}>
                <TypingDot delay={0} /><TypingDot delay={0.18} /><TypingDot delay={0.36} />
              </div>
            </div>
          );
          if (msg.kind === 'bot') return (
            <div key={i} className="fade-up" style={{ display: 'flex', alignItems: 'flex-start', gap: 7 }}>
              <div style={{ width: 26, height: 26, borderRadius: '50%', background: '#128C7E', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12, flexShrink: 0 }}>🤖</div>
              <div style={{ background: '#1A2236', borderRadius: '2px 10px 10px 10px', padding: '9px 12px', fontSize: 12, color: '#EAEDF5', lineHeight: 1.6, maxWidth: '82%', whiteSpace: 'pre-line' }}>
                {msg.text}<div style={{ fontSize: 9, color: '#2D3748', marginTop: 5 }}>09:41</div>
              </div>
            </div>
          );
          if (msg.kind === 'pdf') return (
            <div key={i} className="fade-up" style={{ display: 'flex', alignItems: 'flex-start', gap: 7 }}>
              <div style={{ width: 26, height: 26, borderRadius: '50%', background: '#128C7E', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12, flexShrink: 0 }}>🤖</div>
              <div style={{ background: '#1A2236', borderRadius: '2px 10px 10px 10px', padding: '11px 12px', minWidth: 195 }}>
                <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
                  <div style={{ width: 34, height: 42, background: '#DC2626', borderRadius: 5, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 10, fontWeight: 700, color: 'white', flexShrink: 0 }}>PDF</div>
                  <div>
                    <div style={{ fontSize: 12, fontWeight: 600, color: '#EAEDF5', lineHeight: 1.3 }}>{msg.pdfName}</div>
                    <div style={{ fontSize: 10, color: '#4B5563', marginTop: 2 }}>{msg.pdfSize} · Appuyer pour ouvrir</div>
                  </div>
                </div>
                <div style={{ fontSize: 9, color: '#2D3748', marginTop: 6, textAlign: 'right' }}>09:41</div>
              </div>
            </div>
          );
          return null;
        })}
      </div>

      {/* Input */}
      <div style={{ background: '#0F1624', padding: '8px 10px', display: 'flex', alignItems: 'center', gap: 7, borderTop: '1px solid rgba(255,255,255,0.04)' }}>
        <div style={{ flex: 1, background: '#1A2236', borderRadius: 20, padding: '9px 14px', fontSize: 12, color: '#2D3748' }}>Envoyer un message...</div>
        <div style={{ width: 34, height: 34, borderRadius: '50%', background: '#25D366', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14, cursor: 'pointer' }}>↑</div>
      </div>

      {/* Home bar */}
      <div style={{ background: '#0A0F1A', height: 28, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <div style={{ width: 72, height: 4, background: '#1C2333', borderRadius: 2 }} />
      </div>
    </div>
  );
}
