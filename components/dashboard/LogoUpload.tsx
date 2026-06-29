'use client';
import { useState, useRef } from 'react';

export function LogoUpload() {
  const [uploaded, setUploaded] = useState(false);
  const [fileName, setFileName] = useState('');
  const inputRef = useRef<HTMLInputElement>(null);

  function handleFile(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (file) { setFileName(file.name); setUploaded(true); }
  }

  return (
    <div style={{ background: 'var(--card)', border: '1px solid var(--border)', borderRadius: 14, padding: 24, boxShadow: 'var(--shadow)' }}>
      <div style={{ fontSize: 15, fontWeight: 600, marginBottom: 3 }}>Logo de ton entreprise</div>
      <div style={{ fontSize: 13, color: 'var(--muted)', marginBottom: 16 }}>Apparaîtra sur toutes tes factures PDF</div>
      <input ref={inputRef} type="file" accept="image/*" style={{ display: 'none' }} onChange={handleFile} />
      {uploaded ? (
        <div className="fade-up" style={{ border: '2px solid rgba(37,211,102,0.25)', borderRadius: 12, padding: 24, textAlign: 'center', background: 'var(--green-bg)' }}>
          <div style={{ fontSize: 36, marginBottom: 8 }}>✅</div>
          <div style={{ fontSize: 14, fontWeight: 600, color: 'var(--green-text)' }}>Logo uploadé !</div>
          <div style={{ fontSize: 12, color: 'var(--muted)', marginTop: 4 }}>{fileName}</div>
          <button onClick={() => setUploaded(false)} style={{ marginTop: 12, fontSize: 12, color: 'var(--muted)', background: 'transparent', border: '1px solid var(--border)', borderRadius: 7, padding: '6px 14px', cursor: 'pointer', fontFamily: 'Space Grotesk, sans-serif' }}>Changer</button>
        </div>
      ) : (
        <div onClick={() => inputRef.current?.click()} style={{ border: '2px dashed var(--border)', borderRadius: 12, padding: 30, textAlign: 'center', cursor: 'pointer', transition: 'border-color 0.2s' }}>
          <div style={{ fontSize: 26, marginBottom: 8, opacity: 0.35 }}>📁</div>
          <div style={{ fontSize: 14, fontWeight: 500, color: 'var(--muted)' }}>Glisse ton logo ici</div>
          <div style={{ fontSize: 12, color: 'var(--dim)', marginTop: 3 }}>PNG, JPG, SVG · Max 2 MB</div>
          <div style={{ marginTop: 14, display: 'inline-block', fontSize: 13, fontWeight: 600, background: 'var(--bg2)', border: '1px solid var(--border)', borderRadius: 8, padding: '8px 18px', color: 'var(--text2)' }}>Choisir un fichier</div>
        </div>
      )}
    </div>
  );
}
