interface PipelineProps { step: number; }

const STEPS = [
  { icon: '📥', title: 'Webhook .NET 8 reçoit le message', sub: 'Azure Functions · Meta Cloud API' },
  { icon: '🧠', title: 'NLP extrait montant + client',       sub: 'Azure OpenAI · GPT-4o mini' },
  { icon: '📄', title: 'QuestPDF génère le document',        sub: 'Cosmos DB · Blob Storage' },
  { icon: '📤', title: 'WhatsApp Media Message envoyé',      sub: 'Meta Cloud API · <15 secondes' },
];

export function TechnicalPipeline({ step }: PipelineProps) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 7 }}>
      {STEPS.map((s, i) => {
        const active = step >= i + 1;
        return (
          <div key={i} style={{
            display: 'flex', alignItems: 'center', gap: 12, padding: '11px 14px',
            background: active ? 'rgba(37,211,102,0.06)' : 'transparent',
            border: `1px solid ${active ? 'rgba(37,211,102,0.2)' : 'rgba(255,255,255,0.05)'}`,
            borderRadius: 10, transition: 'background 0.3s, border-color 0.3s',
          }}>
            <span style={{ fontSize: 15, flexShrink: 0 }}>{s.icon}</span>
            <div style={{ flex: 1, minWidth: 0 }}>
              <div style={{ fontSize: 13, fontWeight: 500, color: active ? 'var(--text)' : 'var(--dim)' }}>{s.title}</div>
              <div style={{ fontSize: 11, color: 'var(--dim)', marginTop: 1 }}>{s.sub}</div>
            </div>
            {active && (
              <div style={{ width: 8, height: 8, borderRadius: '50%', background: '#25D366', flexShrink: 0, animation: 'pulse 1.5s ease infinite' }} />
            )}
          </div>
        );
      })}
    </div>
  );
}
