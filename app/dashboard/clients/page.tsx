'use client';

import { useCallback, useEffect, useState } from 'react';
import Link from 'next/link';
import {
  createEndClient,
  deleteEndClient,
  fetchEndClients,
  type EndClientInput,
} from '@/lib/piecebot/api';
import { formatShortDate, initials } from '@/lib/piecebot/format';
import type { EndClient } from '@/lib/piecebot/types';

const ACCENT = '#25D366';

const inputStyle: React.CSSProperties = {
  width: '100%',
  background: 'var(--bg)',
  border: '1px solid var(--border)',
  borderRadius: 8,
  padding: '9px 12px',
  fontSize: 13,
  color: 'var(--text)',
  fontFamily: 'inherit',
};

const EMPTY: EndClientInput & { tagsText: string } = {
  companyName: '',
  contactName: '',
  whatsappNumber: '',
  email: '',
  tagsText: '',
};

export default function ClientsPage() {
  const [clients, setClients] = useState<EndClient[]>([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [submitting, setSubmitting] = useState(false);

  const load = useCallback(async (q: string) => {
    setLoading(true);
    setError(null);
    try {
      setClients(await fetchEndClients(q || undefined));
    } catch {
      setError('API injoignable — lancez apps/api (dotnet run).');
      setClients([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    const t = setTimeout(() => load(search), 200);
    return () => clearTimeout(t);
  }, [search, load]);

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await createEndClient({
        companyName: form.companyName,
        contactName: form.contactName,
        whatsappNumber: form.whatsappNumber,
        email: form.email || undefined,
        tags: form.tagsText
          .split(',')
          .map((t) => t.trim())
          .filter(Boolean),
      });
      setForm(EMPTY);
      setShowForm(false);
      await load(search);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de la création');
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete(id: string) {
    if (!confirm('Supprimer ce client ?')) return;
    try {
      await deleteEndClient(id);
      await load(search);
    } catch {
      setError('Suppression impossible.');
    }
  }

  return (
    <main
      style={{
        minHeight: '100vh',
        background: 'var(--bg)',
        color: 'var(--text)',
        fontFamily: 'Space Grotesk, sans-serif',
      }}
    >
      <div style={{ maxWidth: 1000, margin: '0 auto', padding: '32px 28px 48px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 24 }}>
          <div>
            <Link href="/dashboard" style={{ fontSize: 13, color: 'var(--muted)', textDecoration: 'none' }}>
              ← Tableau de bord
            </Link>
            <h1 style={{ fontSize: 24, fontWeight: 700, letterSpacing: '-0.03em', margin: '8px 0 2px' }}>
              Clients du cabinet
            </h1>
            <p style={{ fontSize: 13, color: 'var(--muted)' }}>
              {clients.length} entreprise{clients.length > 1 ? 's' : ''} suivie{clients.length > 1 ? 's' : ''}
            </p>
          </div>
          <button
            onClick={() => {
              setShowForm((v) => !v);
              setError(null);
            }}
            style={{
              background: showForm ? 'var(--card)' : ACCENT,
              color: showForm ? 'var(--text)' : '#000',
              border: showForm ? '1px solid var(--border)' : 'none',
              fontWeight: 700,
              fontSize: 14,
              padding: '10px 18px',
              borderRadius: 10,
              cursor: 'pointer',
              fontFamily: 'inherit',
            }}
          >
            {showForm ? 'Annuler' : '+ Nouveau client'}
          </button>
        </div>

        {showForm && (
          <form
            onSubmit={handleCreate}
            style={{
              background: 'var(--card)',
              border: '1px solid var(--border)',
              borderRadius: 14,
              padding: 20,
              marginBottom: 20,
              display: 'grid',
              gridTemplateColumns: '1fr 1fr',
              gap: 12,
            }}
          >
            <label style={{ fontSize: 12, color: 'var(--muted)' }}>
              Entreprise *
              <input style={inputStyle} value={form.companyName} required
                onChange={(e) => setForm({ ...form, companyName: e.target.value })} placeholder="SARL Boutique Aïssa" />
            </label>
            <label style={{ fontSize: 12, color: 'var(--muted)' }}>
              Contact *
              <input style={inputStyle} value={form.contactName} required
                onChange={(e) => setForm({ ...form, contactName: e.target.value })} placeholder="Aïssa Diallo" />
            </label>
            <label style={{ fontSize: 12, color: 'var(--muted)' }}>
              Numéro WhatsApp *
              <input style={inputStyle} value={form.whatsappNumber} required
                onChange={(e) => setForm({ ...form, whatsappNumber: e.target.value })} placeholder="+237699123456" />
            </label>
            <label style={{ fontSize: 12, color: 'var(--muted)' }}>
              Email
              <input style={inputStyle} value={form.email}
                onChange={(e) => setForm({ ...form, email: e.target.value })} placeholder="contact@entreprise.cm" />
            </label>
            <label style={{ fontSize: 12, color: 'var(--muted)', gridColumn: '1 / -1' }}>
              Tags (séparés par des virgules)
              <input style={inputStyle} value={form.tagsText}
                onChange={(e) => setForm({ ...form, tagsText: e.target.value })} placeholder="commerce, actif" />
            </label>
            {error && <div style={{ gridColumn: '1 / -1', color: '#EF4444', fontSize: 13 }}>{error}</div>}
            <button type="submit" disabled={submitting}
              style={{
                gridColumn: '1 / -1', justifySelf: 'start', background: ACCENT, color: '#000', border: 'none',
                fontWeight: 700, fontSize: 14, padding: '10px 18px', borderRadius: 10,
                cursor: submitting ? 'wait' : 'pointer', fontFamily: 'inherit', opacity: submitting ? 0.6 : 1,
              }}>
              {submitting ? 'Création…' : 'Créer le client'}
            </button>
          </form>
        )}

        <input value={search} onChange={(e) => setSearch(e.target.value)}
          placeholder="Rechercher par nom, contact, numéro, tag…" style={{ ...inputStyle, marginBottom: 16 }} />

        <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, overflow: 'hidden' }}>
          {loading ? (
            <div style={{ padding: 24, fontSize: 13, color: 'var(--muted)' }}>Chargement…</div>
          ) : error && clients.length === 0 ? (
            <div style={{ padding: 24, fontSize: 13, color: '#F59E0B' }}>{error}</div>
          ) : clients.length === 0 ? (
            <div style={{ padding: 24, fontSize: 13, color: 'var(--muted)' }}>
              Aucun client. Ajoute ton premier client avec « + Nouveau client ».
            </div>
          ) : (
            clients.map((c) => (
              <div key={c.id}
                style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '14px 16px', borderTop: '1px solid var(--border)' }}>
                <div style={{
                  width: 36, height: 36, borderRadius: 9, background: `${ACCENT}1A`, color: ACCENT,
                  display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12, fontWeight: 700, flexShrink: 0,
                }}>
                  {initials(c.companyName)}
                </div>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <Link href={`/dashboard/clients/${c.id}`}
                    style={{ fontSize: 14, fontWeight: 600, color: 'var(--text)', textDecoration: 'none' }}>
                    {c.companyName}
                  </Link>
                  <div style={{ fontSize: 12, color: 'var(--muted)' }}>{c.contactName} · {c.whatsappNumber}</div>
                </div>
                <div style={{ fontSize: 11, color: 'var(--dim)', textAlign: 'right' }}>
                  {c.lastPieceReceived ? `Dernière pièce ${formatShortDate(c.lastPieceReceived)}` : 'Aucune pièce'}
                </div>
                <button onClick={() => handleDelete(c.id)} title="Supprimer"
                  style={{
                    background: 'transparent', border: '1px solid var(--border)', color: '#EF4444',
                    borderRadius: 8, padding: '6px 10px', fontSize: 12, cursor: 'pointer', fontFamily: 'inherit',
                  }}>
                  Suppr.
                </button>
              </div>
            ))
          )}
        </div>
      </div>
    </main>
  );
}
