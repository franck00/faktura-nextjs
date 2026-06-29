'use client';
import { useLang } from '@/hooks/useLang';

export function LangSwitcher() {
  const { lang, setLang } = useLang();
  const btn = (l: 'fr' | 'en') => ({
    padding: '5px 11px', borderRadius: 6, border: 'none',
    cursor: 'pointer', fontFamily: 'Space Grotesk, sans-serif',
    fontSize: 13, fontWeight: 600, transition: 'background 0.15s',
    background: lang === l ? 'var(--card)' : 'transparent',
    color:      lang === l ? 'var(--text)' : 'var(--muted)',
  } as React.CSSProperties);
  return (
    <div style={{
      display: 'flex', alignItems: 'center', gap: 3,
      background: 'var(--bg2)', border: '1px solid var(--border)',
      borderRadius: 8, padding: 3,
    }}>
      <button style={btn('fr')} onClick={() => setLang('fr')}>FR</button>
      <button style={btn('en')} onClick={() => setLang('en')}>EN</button>
    </div>
  );
}
