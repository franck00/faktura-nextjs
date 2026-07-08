'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { deleteEndClient, fetchEndClient, updateEndClient } from '@/lib/piecebot/api';
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

const wrap: React.CSSProperties = {
  minHeight: '100vh',
  background: 'var(--bg)',
  color: 'var(--text)',
  fontFamily: 'Space Grotesk, sans-serif',
};

export default function ClientDetailPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [client, setClient] = useState<EndClient | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        setClient(await fetchEndClient(id));
      } catch (err) {
        setNotFound(true);
        setError(err instanceof Error ? err.message : null);
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  async function handleSave(e: React.FormEvent) {
    e.preventDefault();
    if (!client) return;
    setError(null);
    setSaved(false);
    setSaving(true);
    try {
      const updated = await updateEndClient(id, {
        companyName: client.companyName,
        contactName: client.contactName,
        whatsappNumber: client.whatsappNumber,
        email: client.email ?? undefined,
        siret: client.siret ?? undefined,
        vatNumber: client.vatNumber ?? undefined,
        tags: client.tags,
      });
      setClient(updated);
      setSaved(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erreur lors de l’enregistrement');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!confirm('Supprimer ce client ?')) return;
    await deleteEndClient(id);
    router.push('/dashboard/clients');
  }

  if (loading) {
    return <main style={wrap}><div style={{ maxWidth: 720, margin: '0 auto', padding: 32, color: 'var(--muted)' }}>Chargement…</div></main>;
  }

  if (notFound || !client) {
    return (
      <main style={wrap}>
        <div style={{ maxWidth: 720, margin: '0 auto', padding: 32 }}>
          <p style={{ color: 'var(--muted)', marginBottom: 16 }}>{error ?? 'Client introuvable.'}</p>
          <Link href="/dashboard/clients" style={{ color: ACCENT }}>← Retour aux clients</Link>
        </div>
      </main>
    );
  }

  return (
    <main style={wrap}>
      <div style={{ maxWidth: 720, margin: '0 auto', padding: '32px 28px 48px' }}>
        <Link href="/dashboard/clients" style={{ fontSize: 13, color: 'var(--muted)', textDecoration: 'none' }}>← Clients</Link>
        <h1 style={{ fontSize: 24, fontWeight: 700, letterSpacing: '-0.03em', margin: '10px 0 24px' }}>{client.companyName}</h1>

        <form onSubmit={handleSave}
          style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, padding: 20, display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 14 }}>
          <label style={{ fontSize: 12, color: 'var(--muted)' }}>
            Entreprise *
            <input style={inputStyle} value={client.companyName} required
              onChange={(e) => setClient({ ...client, companyName: e.target.value })} />
          </label>
          <label style={{ fontSize: 12, color: 'var(--muted)' }}>
            Contact *
            <input style={inputStyle} value={client.contactName} required
              onChange={(e) => setClient({ ...client, contactName: e.target.value })} />
          </label>
          <label style={{ fontSize: 12, color: 'var(--muted)' }}>
            Numéro WhatsApp *
            <input style={inputStyle} value={client.whatsappNumber} required
              onChange={(e) => setClient({ ...client, whatsappNumber: e.target.value })} />
          </label>
          <label style={{ fontSize: 12, color: 'var(--muted)' }}>
            Email
            <input style={inputStyle} value={client.email ?? ''}
              onChange={(e) => setClient({ ...client, email: e.target.value || null })} />
          </label>
          <label style={{ fontSize: 12, color: 'var(--muted)' }}>
            SIRET / NIU
            <input style={inputStyle} value={client.siret ?? ''}
              onChange={(e) => setClient({ ...client, siret: e.target.value || null })} />
          </label>
          <label style={{ fontSize: 12, color: 'var(--muted)' }}>
            N° TVA
            <input style={inputStyle} value={client.vatNumber ?? ''}
              onChange={(e) => setClient({ ...client, vatNumber: e.target.value || null })} />
          </label>
          <label style={{ fontSize: 12, color: 'var(--muted)', gridColumn: '1 / -1' }}>
            Tags (séparés par des virgules)
            <input style={inputStyle} value={client.tags.join(', ')}
              onChange={(e) => setClient({ ...client, tags: e.target.value.split(',').map((t) => t.trim()).filter(Boolean) })} />
          </label>

          {error && <div style={{ gridColumn: '1 / -1', color: '#EF4444', fontSize: 13 }}>{error}</div>}
          {saved && !error && <div style={{ gridColumn: '1 / -1', color: ACCENT, fontSize: 13 }}>✓ Modifications enregistrées</div>}

          <div style={{ gridColumn: '1 / -1', display: 'flex', justifyContent: 'space-between', marginTop: 4 }}>
            <button type="submit" disabled={saving}
              style={{ background: ACCENT, color: '#000', border: 'none', fontWeight: 700, fontSize: 14, padding: '10px 18px', borderRadius: 10, cursor: saving ? 'wait' : 'pointer', fontFamily: 'inherit', opacity: saving ? 0.6 : 1 }}>
              {saving ? 'Enregistrement…' : 'Enregistrer'}
            </button>
            <button type="button" onClick={handleDelete}
              style={{ background: 'transparent', border: '1px solid var(--border)', color: '#EF4444', fontWeight: 600, fontSize: 14, padding: '10px 18px', borderRadius: 10, cursor: 'pointer', fontFamily: 'inherit' }}>
              Supprimer
            </button>
          </div>
        </form>
      </div>
    </main>
  );
}
