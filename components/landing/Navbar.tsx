'use client';
import { ThemeToggle } from '@/components/ui/ThemeToggle';
import { LangSwitcher } from '@/components/ui/LangSwitcher';
import { useLang } from '@/hooks/useLang';

export function Navbar() {
  const { t } = useLang();
  return (
    <nav style={{
      position: 'sticky', top: 0, zIndex: 100,
      background: 'var(--nav-bg)',
      backdropFilter: 'blur(20px)', WebkitBackdropFilter: 'blur(20px)',
      borderBottom: '1px solid var(--border)',
      transition: 'background 0.3s',
    }}>
      <div style={{
        maxWidth: 1200, margin: '0 auto', height: 62, padding: '0 32px',
        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 9 }}>
          <div style={{
            width: 32, height: 32, background: '#25D366', borderRadius: 8,
            display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 16,
          }}>📄</div>
          <span style={{ fontSize: 18, fontWeight: 700, letterSpacing: '-0.03em' }}>Faktura</span>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 24 }}>
          <a href="#how"     style={{ fontSize: 14, color: 'var(--muted)', fontWeight: 500 }}>{t.howLabel}</a>
          <a href="#pricing" style={{ fontSize: 14, color: 'var(--muted)', fontWeight: 500 }}>Tarifs</a>
          <LangSwitcher />
          <ThemeToggle />
          <a href="#waitlist" style={{
            background: '#25D366', color: '#000', fontWeight: 700,
            fontSize: 14, padding: '9px 20px', borderRadius: 9,
          }}>{t.navCTA}</a>
        </div>
      </div>
    </nav>
  );
}
