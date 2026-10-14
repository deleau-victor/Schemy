import { cleanup } from '@testing-library/react';
import { afterEach } from 'vite-plus/test';

// Vitest globals are off: unmount rendered trees explicitly between tests.
afterEach(() => {
  cleanup();
  localStorage.clear();
});
