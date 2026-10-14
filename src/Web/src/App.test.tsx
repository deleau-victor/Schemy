import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vite-plus/test';
import { App } from './App';

describe('App', () => {
  it('affiche le nom de l’application', () => {
    render(<App />);
    expect(screen.getByText('Schemy')).toBeTruthy();
  });
});
