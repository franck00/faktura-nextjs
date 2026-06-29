'use client';
import { useState, useEffect } from 'react';
import type { Theme } from '@/lib/types';

export function useTheme() {
  const [theme, setTheme] = useState<Theme>('light');

  useEffect(() => {
    const stored = localStorage.getItem('faktura-theme') as Theme | null;
    if (stored) apply(stored);
  }, []);

  function apply(t: Theme) {
    setTheme(t);
    document.documentElement.setAttribute('data-theme', t);
    localStorage.setItem('faktura-theme', t);
  }

  return { theme, toggle: () => apply(theme === 'light' ? 'dark' : 'light') };
}
