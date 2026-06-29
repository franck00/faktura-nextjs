import type { Config } from 'tailwindcss';

const config: Config = {
  content: [
    './app/**/*.{ts,tsx}',
    './components/**/*.{ts,tsx}',
    './hooks/**/*.{ts,tsx}',
    './lib/**/*.{ts,tsx}',
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Space Grotesk', 'sans-serif'],
      },
      colors: {
        green: {
          DEFAULT: '#25D366',
          wa:      '#128C7E',
          header:  '#075E54',
        },
      },
      borderRadius: {
        card: '14px',
        card2: '20px',
      },
    },
  },
  plugins: [],
};

export default config;
