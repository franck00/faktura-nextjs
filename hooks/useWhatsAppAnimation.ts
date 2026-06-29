'use client';
import { useState, useEffect, useRef } from 'react';

export function useWhatsAppAnimation() {
  const [step, setStep] = useState(0);
  const timers = useRef<ReturnType<typeof setTimeout>[]>([]);

  function clear() { timers.current.forEach(clearTimeout); timers.current = []; }

  function start() {
    clear();
    const add = (ms: number, s: number) => {
      const id = setTimeout(() => setStep(s), ms);
      timers.current.push(id);
    };
    add(900,  1);
    add(1900, 2);
    add(3200, 3);
    add(4100, 4);
    add(5000, 5);
    timers.current.push(setTimeout(() => { setStep(0); start(); }, 8400));
  }

  useEffect(() => { start(); return clear; }, []);

  return step;
}
