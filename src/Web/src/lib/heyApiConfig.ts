import type { CreateClientConfig } from '@/generated/client.gen';

// Same origin: the Vite dev server proxies /api to the API (vite.config.ts).
// An absolute origin (instead of '') because Request() rejects relative URLs outside a browser (tests).
export const createClientConfig: CreateClientConfig = (config) => ({
  ...config,
  baseUrl: globalThis.location?.origin ?? '',
});
