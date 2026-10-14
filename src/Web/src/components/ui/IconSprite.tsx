import sprite from './icons.svg?raw';

/** Inline sprite (not an external file) so the shared `gShine` gradient resolves in every <use>. */
export function IconSprite() {
  return <div dangerouslySetInnerHTML={{ __html: sprite }} />;
}
