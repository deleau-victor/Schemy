export type UiTheme = 'auto' | 'light' | 'dark';

const DARK_QUERY = '(prefers-color-scheme: dark)';

export function parseUiTheme(raw: string | null): UiTheme {
  return raw === 'light' || raw === 'dark' ? raw : 'auto';
}

export function resolveIsDark(ui: UiTheme, systemPrefersDark: boolean): boolean {
  return ui === 'auto' ? systemPrefersDark : ui === 'dark';
}

/** Same classes as schemy.html: `html:root.ui-light|ui-dark` override the system tokens. */
export function applyUiTheme(root: HTMLElement, ui: UiTheme): void {
  root.classList.toggle('ui-light', ui === 'light');
  root.classList.toggle('ui-dark', ui === 'dark');
}

/** The toggle flips the *effective* theme, like toggleUI() in schemy.html. */
export function nextUiTheme(isDark: boolean): UiTheme {
  return isDark ? 'light' : 'dark';
}

function systemQuery(): MediaQueryList | null {
  return typeof window.matchMedia === 'function' ? window.matchMedia(DARK_QUERY) : null;
}

export function getSystemPrefersDark(): boolean {
  return systemQuery()?.matches ?? false;
}

export function subscribeSystemPrefersDark(onChange: () => void): () => void {
  const query = systemQuery();
  if (!query) return () => {};
  query.addEventListener('change', onChange);
  return () => query.removeEventListener('change', onChange);
}
