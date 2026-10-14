import { afterEach, describe, expect, it, vi } from 'vite-plus/test';
import { readPref, writePref } from './storage';

describe('storage', () => {
  afterEach(() => vi.restoreAllMocks());

  it('préfixe les clés par schemy.', () => {
    writePref('ui', 'dark');
    expect(localStorage.getItem('schemy.ui')).toBe('dark');
    expect(readPref('ui', (raw) => raw)).toBe('dark');
  });

  it('ne lève pas quand localStorage est inaccessible', () => {
    vi.spyOn(Storage.prototype, 'getItem').mockImplementation(() => {
      throw new DOMException('SecurityError');
    });
    vi.spyOn(Storage.prototype, 'setItem').mockImplementation(() => {
      throw new DOMException('QuotaExceededError');
    });

    expect(readPref('ui', (raw) => raw ?? 'défaut')).toBe('défaut');
    expect(() => writePref('ui', 'dark')).not.toThrow();
  });
});
