'use client';
import { useState, useRef, useCallback } from 'react';
import { SCENARIOS } from '@/lib/data';
import type { ScenarioKey, ChatMessage } from '@/lib/types';

export function useBotAnimation() {
  const [scenario, setScenario] = useState<ScenarioKey | null>(null);
  const [step, setStep]         = useState(0);
  const timers = useRef<ReturnType<typeof setTimeout>[]>([]);

  function clear() { timers.current.forEach(clearTimeout); timers.current = []; }

  const play = useCallback((key: ScenarioKey) => {
    clear();
    setScenario(key);
    setStep(0);
    SCENARIOS[key].delays.forEach((ms, i) => {
      const id = setTimeout(() => setStep(i + 1), ms);
      timers.current.push(id);
    });
  }, []);

  const restart = useCallback(() => {
    if (!scenario) return;
    clear();
    setStep(0);
    const id = setTimeout(() => play(scenario), 300);
    timers.current.push(id);
  }, [scenario, play]);

  const messages: ChatMessage[] = scenario
    ? (SCENARIOS[scenario].msgs.slice(0, step) as ChatMessage[])
    : [];

  return { scenario, step, messages, play, restart };
}
