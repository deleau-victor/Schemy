import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it } from 'vite-plus/test';
import { usePrefsStore } from '@/stores/prefsStore';
import { ThemeToggle } from './ThemeToggle';

describe('ThemeToggle', () => {
  beforeEach(() => {
    usePrefsStore.setState({ ui: 'auto' });
    document.documentElement.className = '';
  });

  it('bascule vers le sombre puis vers le clair, et mémorise le choix', async () => {
    render(<ThemeToggle />);

    await userEvent.click(screen.getByRole('button', { name: 'Passer en mode sombre' }));
    expect(document.documentElement.classList.contains('ui-dark')).toBe(true);
    expect(localStorage.getItem('schemy.ui')).toBe('dark');

    await userEvent.click(screen.getByRole('button', { name: 'Passer en mode clair' }));
    expect(document.documentElement.classList.contains('ui-light')).toBe(true);
    expect(localStorage.getItem('schemy.ui')).toBe('light');
  });
});
