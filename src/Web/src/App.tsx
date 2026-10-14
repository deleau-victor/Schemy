import { QueryClientProvider } from '@tanstack/react-query';
import { useState } from 'react';
import { RouterProvider, createBrowserRouter } from 'react-router';
import { IconSprite } from '@/components/ui/IconSprite';
import { createQueryClient } from '@/lib/queryClient';
import { routes } from '@/routes';

export function App() {
  const [queryClient] = useState(createQueryClient);
  const [router] = useState(() => createBrowserRouter(routes));
  return (
    <QueryClientProvider client={queryClient}>
      <IconSprite />
      <RouterProvider router={router} />
    </QueryClientProvider>
  );
}
