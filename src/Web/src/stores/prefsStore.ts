import { create } from 'zustand';
import { readPref, writePref } from '@/lib/prefs/storage';
import { parseUiTheme, type UiTheme } from '@/lib/theme/theme';

type PrefsState = {
  ui: UiTheme;
  setUi: (ui: UiTheme) => void;
};

export const usePrefsStore = create<PrefsState>((set) => ({
  ui: readPref('ui', parseUiTheme),
  setUi: (ui) => {
    writePref('ui', ui);
    set({ ui });
  },
}));
