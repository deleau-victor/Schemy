import { screen } from '@testing-library/react';
import { describe, expect, it } from 'vite-plus/test';
import { renderRoute } from '@/test/renderWithProviders';

describe('routes', () => {
  it.each([
    ['/', 'Mes schémas'],
    ['/recent', 'Récents'],
    ['/starred', 'Favoris'],
    ['/trash', 'Corbeille'],
  ])('%s affiche l’emplacement « %s »', async (path, label) => {
    renderRoute(path);
    expect(await screen.findByRole('heading', { level: 1, name: label })).toBeTruthy();
  });

  it('un lien profond vers un dossier conserve son identifiant', async () => {
    renderRoute('/f/01JABCDEF0123456789XYZ0000');
    expect(await screen.findByText('Dossier 01JABCDEF0123456789XYZ0000')).toBeTruthy();
  });

  it('un lien profond vers un schéma ouvre l’éditeur', async () => {
    renderRoute('/d/01JABCDEF0123456789XYZ0001');
    expect(await screen.findByRole('heading', { level: 1, name: 'Éditeur' })).toBeTruthy();
  });

  it('une URL inconnue affiche « Page introuvable » avec un lien de retour', async () => {
    renderRoute('/n/importe/quoi');
    expect(await screen.findByRole('heading', { level: 1, name: 'Page introuvable' })).toBeTruthy();
    expect(screen.getByRole('link', { name: 'Retour à Mes schémas' }).getAttribute('href')).toBe(
      '/',
    );
  });

  it('le shell porte le nom de l’app et la bascule de thème', async () => {
    renderRoute('/');
    expect(await screen.findByText('Schemy')).toBeTruthy();
    expect(screen.getByRole('button', { name: /Passer en mode/ })).toBeTruthy();
  });
});
