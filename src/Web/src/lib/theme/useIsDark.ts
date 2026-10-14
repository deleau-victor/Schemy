import { useSyncExternalStore } from 'react';
import { usePrefsStore } from '@/stores/prefsStore';
import { getSystemPrefersDark, resolveIsDark, subscribeSystemPrefersDark } from './theme';

export function useIsDark(): boolean {
  const ui = usePrefsStore((s) => s.ui);
  const systemDark = useSyncExternalStore(subscribeSystemPrefersDark, getSystemPrefersDark);
  return resolveIsDark(ui, systemDark);
}
