import { describe, expect, it, vi } from 'vite-plus/test';
import {
  applyUiTheme,
  getSystemPrefersDark,
  nextUiTheme,
  parseUiTheme,
  resolveIsDark,
  subscribeSystemPrefersDark,
} from './theme';

function fakeMatchMedia(initial: boolean) {
  const listeners = new Set<() => void>();
  const mql = {
    matches: initial,
    addEventListener: (_: string, cb: () => void) => listeners.add(cb),
    removeEventListener: (_: string, cb: () => void) => listeners.delete(cb),
  };
  vi.stubGlobal('matchMedia', () => mql);
  return {
    flip(value: boolean) {
      mql.matches = value;
      listeners.forEach((cb) => cb());
    },
    listenerCount: () => listeners.size,
  };
}

describe('theme', () => {
  it('accepte light/dark et retombe sur auto sinon', () => {
    expect(parseUiTheme('light')).toBe('light');
    expect(parseUiTheme('dark')).toBe('dark');
    expect(parseUiTheme('violet')).toBe('auto');
    expect(parseUiTheme(null)).toBe('auto');
  });

  it('auto suit le système, un choix explicite l’emporte', () => {
    expect(resolveIsDark('auto', true)).toBe(true);
    expect(resolveIsDark('auto', false)).toBe(false);
    expect(resolveIsDark('light', true)).toBe(false);
    expect(resolveIsDark('dark', false)).toBe(true);
  });

  it('pose une seule classe ui-* sur la racine', () => {
    const root = document.createElement('html');
    applyUiTheme(root, 'dark');
    expect(root.className).toBe('ui-dark');
    applyUiTheme(root, 'light');
    expect(root.className).toBe('ui-light');
    applyUiTheme(root, 'auto');
    expect(root.className).toBe('');
  });

  it('la bascule inverse le thème effectif (comme toggleUI)', () => {
    expect(nextUiTheme(true)).toBe('light');
    expect(nextUiTheme(false)).toBe('dark');
  });

  it('notifie quand le thème système change, et se désabonne', () => {
    const media = fakeMatchMedia(false);
    const onChange = vi.fn();
    const unsubscribe = subscribeSystemPrefersDark(onChange);

    media.flip(true);
    expect(onChange).toHaveBeenCalledTimes(1);
    expect(getSystemPrefersDark()).toBe(true);

    unsubscribe();
    expect(media.listenerCount()).toBe(0);
    vi.unstubAllGlobals();
  });

  it('sans matchMedia (jsdom), considère le système comme clair', () => {
    vi.stubGlobal('matchMedia', undefined);
    expect(getSystemPrefersDark()).toBe(false);
    expect(() => subscribeSystemPrefersDark(() => {})()).not.toThrow();
    vi.unstubAllGlobals();
  });
});
