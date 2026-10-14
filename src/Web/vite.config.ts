import react from '@vitejs/plugin-react';
import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite-plus';

// Injected by AspireHost (spec § 3.4). Defaults allow `vp dev` without Aspire.
const apiUrl = process.env.API_URL ?? 'http://localhost:7010';
const toApi = { target: apiUrl, changeOrigin: true };

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) },
  },
  css: {
    modules: { localsConvention: 'camelCase' },
  },
  server: {
    port: Number(process.env.PORT ?? 5180),
    strictPort: true,
    proxy: { '/api': toApi, '/openapi': toApi, '/swagger': toApi },
  },
  test: {
    environment: 'jsdom',
    setupFiles: ['./vitest.setup.ts'],
    css: true,
  },
  lint: {
    ignorePatterns: ['dist/**', 'src/generated/**'],
    options: { typeAware: true, typeCheck: true },
  },
  fmt: {
    // tokens.css / base.css are extracted verbatim from schemy.html (parity reference).
    ignorePatterns: ['dist/**', 'src/generated/**', 'src/styles/tokens.css', 'src/styles/base.css'],
    singleQuote: true,
  },
  run: {
    tasks: {
      'dotnet-test': { command: 'cd ../.. && dotnet test Schemy.slnx', cache: false },
      ci: {
        command: ['vp check', 'vp test', 'vp build'],
        dependsOn: ['dotnet-test'],
        cache: false,
      },
    },
  },
});
