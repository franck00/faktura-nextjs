'use client';
import { useTheme } from '@/hooks/useTheme';

export function ThemeToggle() {
  const { theme, toggle } = useTheme();
  return (
    <button
      onClick={toggle}
      title="Changer le thème"
      style={{
        width: 38, height: 38, borderRadius: 9,
        border: '1px solid var(--border)',
        background: 'var(--card)',
        cursor: 'pointer', fontSize: 17,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        color: 'var(--muted)', transition: 'background 0.2s',
      }}
    >
      {theme === 'dark' ? '☀️' : '🌙'}
    </button>
  );
}
