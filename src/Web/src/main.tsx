import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { App } from './App';
import { applyUiTheme } from './lib/theme/theme';
import { usePrefsStore } from './stores/prefsStore';
import './styles/tokens.css';
import './styles/base.css';

applyUiTheme(document.documentElement, usePrefsStore.getState().ui);

const container = document.getElementById('root');
if (!container) throw new Error('#root introuvable dans index.html');

createRoot(container).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
