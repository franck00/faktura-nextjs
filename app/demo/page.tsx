'use client';
import { PhoneMockup }       from '@/components/demo/PhoneMockup';
import { ScenarioSelector }  from '@/components/demo/ScenarioSelector';
import { TechnicalPipeline } from '@/components/demo/TechnicalPipeline';
import { useBotAnimation }   from '@/hooks/useBotAnimation';
import type { ScenarioKey }  from '@/lib/types';

export default function DemoPage() {
  const { scenario, step, messages, play, restart } = useBotAnimation();
  const totalSteps = scenario ? messages.length + 1 : 4;

  return (
    <div style={{ minHeight: '100vh', background: '#080C14', display: 'flex', flexDirection: 'column', alignItems: 'center', padding: '52px 32px 64px' }}>
      {/* Header */}
      <div style={{ textAlign: 'center', marginBottom: 48 }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 8, marginBottom: 12 }}>
          <div style={{ width: 28, height: 28, background: '#25D366', borderRadius: 7, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14 }}>📄</div>
          <span style={{ fontSize: 16, fontWeight: 700, letterSpacing: '-0.03em', color: '#EAEDF5' }}>Faktura</span>
        </div>
        <h1 style={{ fontSize: 'clamp(28px,4vw,46px)', fontWeight: 700, letterSpacing: '-0.035em', marginBottom: 12, color: '#EAEDF5' }}>Vois le bot en action.</h1>
        <p style={{ fontSize: 16, color: '#4B5563', maxWidth: 440, margin: '0 auto', lineHeight: 1.65 }}>Sélectionne un scénario pour simuler une conversation complète avec Faktura Bot.</p>
      </div>

      {/* Two-column */}
      <div style={{ display: 'flex', gap: 56, alignItems: 'flex-start', maxWidth: 1020, width: '100%' }}>
        <div style={{ flex: '0 0 300px' }}>
          <PhoneMockup messages={messages} hasStarted={!!scenario} />
        </div>
        <div style={{ flex: 1, minWidth: 0, paddingTop: 4 }}>
          <p style={{ fontSize: 11, color: '#374151', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.1em', marginBottom: 12 }}>Sélectionne un scénario</p>
          <div style={{ marginBottom: 28 }}>
            <ScenarioSelector current={scenario} onSelect={(k: ScenarioKey) => play(k)} />
          </div>

          <p style={{ fontSize: 11, color: '#374151', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.1em', marginBottom: 12 }}>Pipeline technique</p>
          <div style={{ marginBottom: 24 }}>
            <TechnicalPipeline step={step} />
          </div>

          {scenario && (
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 12 }}>
              <div style={{ display: 'flex', gap: 5, alignItems: 'center' }}>
                {Array.from({ length: totalSteps }).map((_, i) => (
                  <div key={i} style={{ height: 6, borderRadius: 3, background: i < step ? '#25D366' : 'rgba(255,255,255,0.1)', width: i < step ? 18 : 6, transition: 'width 0.2s, background 0.2s' }} />
                ))}
              </div>
              <button onClick={restart} style={{ fontSize: 13, fontWeight: 500, color: '#4B5563', background: 'transparent', border: '1px solid rgba(255,255,255,0.08)', borderRadius: 8, padding: '8px 16px', cursor: 'pointer', fontFamily: 'Space Grotesk, sans-serif' }}>↺ Recommencer</button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
