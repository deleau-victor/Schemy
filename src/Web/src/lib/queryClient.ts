import { QueryClient } from '@tanstack/react-query';

export function createQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      // Spec § 6.4: no realtime, refetch when the window regains focus.
      queries: { refetchOnWindowFocus: true, staleTime: 10_000, retry: 1 },
    },
  });
}
