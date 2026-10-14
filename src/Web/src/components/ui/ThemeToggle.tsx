import { useEffect } from 'react';
import { applyUiTheme, nextUiTheme } from '@/lib/theme/theme';
import { useIsDark } from '@/lib/theme/useIsDark';
import { usePrefsStore } from '@/stores/prefsStore';
import { Icon } from './Icon';

export function ThemeToggle() {
  const ui = usePrefsStore((s) => s.ui);
  const setUi = usePrefsStore((s) => s.setUi);
  const isDark = useIsDark();

  useEffect(() => applyUiTheme(document.documentElement, ui), [ui]);

  const label = isDark ? 'Passer en mode clair' : 'Passer en mode sombre';
  return (
    <button
      type="button"
      className="icon-btn"
      aria-label={label}
      title={isDark ? 'Mode clair' : 'Mode sombre'}
      onClick={() => setUi(nextUiTheme(isDark))}
    >
      <Icon name={isDark ? 'i-sun' : 'i-moon'} />
    </button>
  );
}
