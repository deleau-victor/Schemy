import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vite-plus/test';
import { App } from './App';

describe('App', () => {
  it('monte le routeur, le sprite et la page d’accueil', async () => {
    const { container } = render(<App />);
    expect(await screen.findByRole('heading', { level: 1, name: 'Mes schémas' })).toBeTruthy();
    expect(container.querySelector('symbol#i-home')).toBeTruthy();
  });
});
