const PREFIX = 'schemy.';

// localStorage can be missing or throw (private mode, blocked site data): prefs are a convenience, never fatal.
export function readPref<T>(key: string, parse: (raw: string | null) => T): T {
  let raw: string | null = null;
  try {
    raw = localStorage.getItem(PREFIX + key);
  } catch {
    raw = null;
  }
  return parse(raw);
}

export function writePref(key: string, value: string): void {
  try {
    localStorage.setItem(PREFIX + key, value);
  } catch {
    // Ignored on purpose: the in-memory store still holds the value for this session.
  }
}
