import { render } from '@testing-library/react';
import { describe, expect, it } from 'vite-plus/test';
import { ICON_NAMES, Icon } from './Icon';
import sprite from './icons.svg?raw';

describe('Icon', () => {
  it('chaque nom d’icône existe dans le sprite', () => {
    const ids = [...sprite.matchAll(/<symbol id="([^"]+)"/g)].map((m) => m[1]);
    expect([...ICON_NAMES].sort()).toEqual([...ids].sort());
  });

  it('référence le symbole', () => {
    const { container } = render(<Icon name="i-star" />);
    expect(container.querySelector('use')?.getAttribute('href')).toBe('#i-star');
    expect(container.querySelector('svg')?.getAttribute('class')).toBe('i');
  });
});
