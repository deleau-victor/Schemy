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
    host: true,
    port: Number(process.env.PORT ?? 5180),
    strictPort: true,
    allowedHosts: ['host.containers.internal'],
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
    ignorePatterns: ['dist/**', 'src/generated/**'],
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
