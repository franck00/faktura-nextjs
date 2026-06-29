import { Navbar }          from '@/components/landing/Navbar';
import { HeroSection }      from '@/components/landing/HeroSection';
import { StatsStrip }       from '@/components/landing/StatsStrip';
import { HowItWorks }       from '@/components/landing/HowItWorks';
import { PricingSection }   from '@/components/landing/PricingSection';
import { WaitlistSection }  from '@/components/landing/WaitlistSection';
import { Footer }           from '@/components/landing/Footer';

/* Competition table (inline — simple enough) */
function CompetitionTable() {
  const cols = [
    { name: 'Invoice Ninja, Wave',   badges: [{ label: 'Web only · Pas WhatsApp', color: '#B45309', bg: 'rgba(251,191,36,0.1)' }] },
    { name: 'QuickBooks, FreshBooks', badges: [{ label: 'Trop cher · PME africaines', color: '#DC2626', bg: 'rgba(239,68,68,0.1)' }] },
    { name: 'InvoiceBot (WA)',        badges: [{ label: 'Existe · UX médiocre', color: '#B45309', bg: 'rgba(251,191,36,0.1)' }] },
    { name: 'Faktura',                badges: [{ label: 'WhatsApp · Multi-devises · IA', color: '#16803D', bg: 'rgba(37,211,102,0.1)' }], green: true },
  ];
  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '0 32px 80px' }}>
      <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 20, padding: '32px 40px', boxShadow: 'var(--shadow)' }}>
        <p style={{ fontSize: 12, color: 'var(--muted)', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.1em', marginBottom: 20 }}>Pourquoi pas les autres ?</p>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4,1fr)' }}>
          {cols.map((c, i) => (
            <div key={c.name} style={{ padding: i === 0 ? '0 24px 0 0' : i === 3 ? '0 0 0 24px' : '0 24px', borderRight: i < 3 ? '1px solid var(--border)' : undefined }}>
              <div style={{ fontSize: 14, fontWeight: 500, color: c.green ? '#16803D' : 'var(--muted)', marginBottom: 8 }}>{c.name}</div>
              {c.badges.map(b => (
                <div key={b.label} style={{ display: 'inline-block', fontSize: 12, padding: '4px 10px', borderRadius: 6, background: b.bg, color: b.color, fontWeight: 500 }}>{b.label}</div>
              ))}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default function LandingPage() {
  return (
    <div style={{ minHeight: '100vh', background: 'var(--bg)', color: 'var(--text)' }}>
      <Navbar />
      <HeroSection />
      <StatsStrip />
      <HowItWorks />
      <CompetitionTable />
      <PricingSection />
      <WaitlistSection />
      <Footer />
    </div>
  );
}
