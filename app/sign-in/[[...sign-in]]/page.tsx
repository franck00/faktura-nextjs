import { SignIn } from '@clerk/nextjs';

export default function SignInPage() {
  return (
    <main
      style={{
        minHeight: '100vh',
        display: 'grid',
        placeItems: 'center',
        padding: 24,
        background: 'var(--bg)',
        fontFamily: 'Space Grotesk, sans-serif',
      }}
    >
      <SignIn />
    </main>
  );
}
