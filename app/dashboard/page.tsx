'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { OrganizationSwitcher, UserButton } from '@clerk/nextjs';
import {
  fetchClientCompletions,
  fetchCurrentTenant,
  fetchDashboardStats,
  fetchEndClients,
  fetchPieces,
  groupPiecesByClient,
  validatePiece,
} from '@/lib/piecebot/api';
import {
  CURRENT_MONTH,
  DEMO_TENANT,
  getClientCompletions,
  getDashboardStats,
  getPiecesByClient,
} from '@/lib/piecebot/mock';
import {
  CATEGORY_LABELS,
  STATUS_META,
  confidenceColor,
  formatAmount,
  formatPercent,
  formatShortDate,
  initials,
} from '@/lib/piecebot/format';
import type { ClientCompletion, DashboardStats, EndClient, Piece } from '@/lib/piecebot/types';

const ACCENT = '#25D366';

/** Contrôles Clerk (org switcher + user button) affichés seulement si Clerk est configuré. */
const CLERK_ENABLED = Boolean(process.env.NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY);

interface DashboardData {
  tenantName: string;
  stats: DashboardStats;
  completions: ClientCompletion[];
  inbox: Array<{ client: EndClient; pieces: Piece[] }>;
  offline: boolean;
}

/** Charge les données depuis l'API .NET ; bascule sur le mock si l'API est injoignable. */
async function loadDashboard(month: string, signal: AbortSignal): Promise<DashboardData> {
  try {
    const [tenant, stats, completions, pieces, clients] = await Promise.all([
      fetchCurrentTenant(signal),
      fetchDashboardStats(month, signal),
      fetchClientCompletions(month, signal),
      fetchPieces(month, signal),
      fetchEndClients(undefined, signal),
    ]);
    return {
      tenantName: tenant.name,
      stats,
      completions,
      inbox: groupPiecesByClient(clients, pieces),
      offline: false,
    };
  } catch {
    // Fallback hors-ligne : données de démonstration locales.
    return {
      tenantName: DEMO_TENANT.name,
      stats: getDashboardStats(month),
      completions: getClientCompletions(month),
      inbox: getPiecesByClient(month),
      offline: true,
    };
  }
}

function KpiCard({ label, value, hint }: { label: string; value: string; hint?: string }) {
  return (
    <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, padding: 20 }}>
      <div style={{ fontSize: 12, color: 'var(--muted)', marginBottom: 8 }}>{label}</div>
      <div style={{ fontSize: 26, fontWeight: 700, letterSpacing: '-0.02em' }}>{value}</div>
      {hint && <div style={{ fontSize: 12, color: 'var(--dim)', marginTop: 4 }}>{hint}</div>}
    </div>
  );
}

function StatusBadge({ status }: { status: Piece['status'] }) {
  const meta = STATUS_META[status];
  return (
    <span
      style={{
        fontSize: 11,
        fontWeight: 600,
        color: meta.color,
        background: `${meta.color}1A`,
        padding: '3px 9px',
        borderRadius: 6,
        whiteSpace: 'nowrap',
      }}
    >
      {meta.label}
    </span>
  );
}

function PieceRow({
  piece,
  onValidate,
  validating,
}: {
  piece: Piece;
  onValidate: (id: string) => void;
  validating: boolean;
}) {
  const { extractedData: ex } = piece;
  return (
    <div
      style={{
        display: 'grid',
        gridTemplateColumns: '1.4fr 1fr 0.9fr 0.7fr auto',
        alignItems: 'center',
        gap: 12,
        padding: '12px 16px',
        borderTop: '1px solid var(--border)',
        fontSize: 13,
      }}
    >
      <div style={{ minWidth: 0 }}>
        <div style={{ fontWeight: 600, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
          {ex.supplier ?? piece.originalFileName}
        </div>
        <div style={{ fontSize: 11, color: 'var(--muted)' }}>{CATEGORY_LABELS[piece.category]}</div>
      </div>
      <div style={{ fontWeight: 600 }}>{formatAmount(ex.totalAmountTtc, ex.currency)}</div>
      <div style={{ color: 'var(--muted)' }}>{formatShortDate(ex.documentDate ?? piece.receivedAt)}</div>
      <div title={`Confiance OCR ${formatPercent(piece.confidence)}`}>
        {piece.status === 'received' || piece.status === 'processing' ? (
          <span style={{ fontSize: 11, color: 'var(--dim)' }}>—</span>
        ) : (
          <span style={{ fontSize: 12, fontWeight: 600, color: confidenceColor(piece.confidence) }}>
            {formatPercent(piece.confidence)}
          </span>
        )}
      </div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, justifyContent: 'flex-end' }}>
        {piece.status === 'extracted' && (
          <button
            onClick={() => onValidate(piece.id)}
            disabled={validating}
            style={{
              background: '#25D366',
              color: '#000',
              border: 'none',
              fontWeight: 700,
              fontSize: 11,
              padding: '4px 10px',
              borderRadius: 6,
              cursor: validating ? 'wait' : 'pointer',
              fontFamily: 'inherit',
              opacity: validating ? 0.6 : 1,
            }}
          >
            {validating ? '…' : 'Valider'}
          </button>
        )}
        <StatusBadge status={piece.status} />
      </div>
    </div>
  );
}

export default function DashboardPage() {
  const [month] = useState(CURRENT_MONTH);
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [refreshKey, setRefreshKey] = useState(0);
  const [validatingId, setValidatingId] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();
    setLoading(true);
    loadDashboard(month, controller.signal).then((result) => {
      setData(result);
      setLoading(false);
    });
    return () => controller.abort();
  }, [month, refreshKey]);

  async function handleValidate(pieceId: string) {
    setValidatingId(pieceId);
    try {
      await validatePiece(pieceId);
      setRefreshKey((k) => k + 1);
    } catch {
      /* API offline (mode démo) : la validation n'est pas persistée. */
    } finally {
      setValidatingId(null);
    }
  }

  const monthLabel = new Intl.DateTimeFormat('fr-FR', { month: 'long', year: 'numeric' }).format(
    new Date(`${month}-01T00:00:00`),
  );

  const wrap: React.CSSProperties = {
    minHeight: '100vh',
    background: 'var(--bg)',
    color: 'var(--text)',
    fontFamily: 'Space Grotesk, sans-serif',
  };

  if (loading || !data) {
    return (
      <main style={wrap}>
        <div style={{ maxWidth: 1100, margin: '0 auto', padding: 32, color: 'var(--muted)' }}>
          Chargement du tableau de bord…
        </div>
      </main>
    );
  }

  const { tenantName, stats, completions, inbox, offline } = data;
  const lateClients = completions.filter((c) => c.piecesReceived === 0);

  return (
    <main style={wrap}>
      <div style={{ maxWidth: 1100, margin: '0 auto', padding: '32px 28px 48px' }}>
        {offline && (
          <div
            style={{
              background: 'rgba(245,158,11,0.08)',
              border: '1px solid rgba(245,158,11,0.3)',
              borderRadius: 10,
              padding: '10px 14px',
              marginBottom: 16,
              fontSize: 12,
              color: '#F59E0B',
            }}
          >
            ⚠ API injoignable — affichage des données de démonstration (lancez <code>apps/api</code>).
          </div>
        )}

        {/* En-tête */}
        <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', marginBottom: 28 }}>
          <div>
            <div style={{ fontSize: 12, color: ACCENT, fontWeight: 600, marginBottom: 4 }}>{tenantName}</div>
            <h1 style={{ fontSize: 24, fontWeight: 700, letterSpacing: '-0.03em', marginBottom: 3 }}>
              Pièces de {monthLabel}
            </h1>
            <p style={{ fontSize: 13, color: 'var(--muted)' }}>
              Boîte de réception des justificatifs reçus par WhatsApp, classés par client.
            </p>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            <Link
              href="/dashboard/clients"
              style={{
                background: 'var(--card)',
                border: '1px solid var(--border)',
                color: 'var(--text)',
                fontWeight: 600,
                fontSize: 13,
                padding: '9px 16px',
                borderRadius: 10,
                textDecoration: 'none',
                whiteSpace: 'nowrap',
              }}
            >
              Gérer les clients →
            </Link>
            {CLERK_ENABLED && (
              <>
                <OrganizationSwitcher
                  hidePersonal
                  afterSelectOrganizationUrl="/dashboard"
                  appearance={{ elements: { rootBox: { display: 'flex' } } }}
                />
                <UserButton afterSignOutUrl="/" />
              </>
            )}
          </div>
        </div>

        {/* KPIs */}
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 14, marginBottom: 28 }}>
          <KpiCard label="Pièces du mois" value={String(stats.totalPieces)} />
          <KpiCard
            label="À valider"
            value={String(stats.pendingValidation)}
            hint={stats.pendingValidation > 0 ? 'En attente du cabinet' : 'Rien en attente'}
          />
          <KpiCard label="Clients actifs" value={`${stats.activeClients}/${completions.length}`} />
          <KpiCard label="Taux de complétion" value={formatPercent(stats.completionRate)} />
        </div>

        {/* Alerte clients en retard */}
        {lateClients.length > 0 && (
          <div
            style={{
              background: 'rgba(245,158,11,0.08)',
              border: '1px solid rgba(245,158,11,0.3)',
              borderRadius: 12,
              padding: '14px 18px',
              marginBottom: 24,
              fontSize: 13,
            }}
          >
            <strong style={{ color: '#F59E0B' }}>⚠ {lateClients.length} client(s) sans pièce</strong>{' '}
            ce mois : {lateClients.map((c) => c.companyName).join(', ')}.
          </div>
        )}

        {/* Inbox par client */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
          {inbox.map(({ client, pieces }) => {
            const completion = completions.find((c) => c.endClientId === client.id);
            return (
              <section
                key={client.id}
                style={{
                  background: 'var(--card)',
                  border: '1px solid var(--border)',
                  borderRadius: 14,
                  overflow: 'hidden',
                }}
              >
                <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '16px 18px' }}>
                  <div
                    style={{
                      width: 38,
                      height: 38,
                      borderRadius: 10,
                      background: `${ACCENT}1A`,
                      color: ACCENT,
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: 13,
                      fontWeight: 700,
                      flexShrink: 0,
                    }}
                  >
                    {initials(client.companyName)}
                  </div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ fontSize: 15, fontWeight: 600 }}>{client.companyName}</div>
                    <div style={{ fontSize: 12, color: 'var(--muted)' }}>
                      {client.contactName} · {client.whatsappNumber}
                    </div>
                  </div>
                  <div style={{ textAlign: 'right' }}>
                    <div style={{ fontSize: 13, fontWeight: 600 }}>
                      {completion?.piecesValidated ?? 0}/{pieces.length} validées
                    </div>
                    <div style={{ fontSize: 11, color: 'var(--dim)' }}>
                      {formatPercent(completion?.completionRate ?? 0)} complété
                    </div>
                  </div>
                </div>
                {pieces.map((piece) => (
                  <PieceRow
                    key={piece.id}
                    piece={piece}
                    onValidate={handleValidate}
                    validating={validatingId === piece.id}
                  />
                ))}
              </section>
            );
          })}
        </div>
      </div>
    </main>
  );
}
