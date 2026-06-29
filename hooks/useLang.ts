'use client';
import { useState, useEffect } from 'react';
import { translations } from '@/lib/i18n';
import type { Lang } from '@/lib/types';

export function useLang() {
  const [lang, setLang] = useState<Lang>('fr');

  useEffect(() => {
    const stored = localStorage.getItem('faktura-lang') as Lang | null;
    if (stored === 'fr' || stored === 'en') setLang(stored);
  }, []);

  function setLanguage(l: Lang) {
    setLang(l);
    localStorage.setItem('faktura-lang', l);
  }

  return { lang, t: translations[lang], setLang: setLanguage };
}
