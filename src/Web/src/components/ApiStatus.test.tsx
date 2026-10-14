import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { render, screen } from '@testing-library/react';
import { afterEach, describe, expect, it, vi } from 'vite-plus/test';
import { client } from '@/generated/client.gen';
import { ApiStatus } from './ApiStatus';

function renderStatus(fetchImpl: typeof fetch) {
  client.setConfig({ fetch: fetchImpl });
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={queryClient}>
      <ApiStatus />
    </QueryClientProvider>,
  );
}

describe('ApiStatus', () => {
  afterEach(() => client.setConfig({ fetch: globalThis.fetch }));

  it('affiche la version quand l’API répond', async () => {
    renderStatus(
      vi.fn(async () =>
        Response.json({ data: { name: 'Schemy', version: '1.2.3' } }),
      ) as unknown as typeof fetch,
    );
    expect(await screen.findByText('API 1.2.3')).toBeTruthy();
  });

  it('affiche « API hors ligne » quand le réseau échoue', async () => {
    renderStatus(
      vi.fn(async () =>
        Promise.reject(new TypeError('Failed to fetch')),
      ) as unknown as typeof fetch,
    );
    expect(await screen.findByText('API hors ligne')).toBeTruthy();
  });

  it('affiche « API hors ligne » sur une erreur 502 du proxy', async () => {
    renderStatus(
      vi.fn(async () => new Response('Bad Gateway', { status: 502 })) as unknown as typeof fetch,
    );
    expect(await screen.findByText('API hors ligne')).toBeTruthy();
  });
});
