export const ICON_NAMES = [
  'i-code',
  'i-pen',
  'i-heart',
  'i-search',
  'i-down',
  'i-grid',
  'i-list',
  'i-zin',
  'i-zout',
  'i-fit',
  'i-x',
  'i-dl',
  'i-more',
  'i-back',
  'i-sun',
  'i-moon',
  'i-folder',
  'i-folder-plus',
  'i-home',
  'i-fwd',
  'i-up',
  'i-dots',
  'i-menu',
  'i-plus',
  'i-clock',
  'i-star',
  'i-trash',
  'i-drive',
  'i-mark',
] as const;

export type IconName = (typeof ICON_NAMES)[number];

type IconProps = { name: IconName; className?: string };

export function Icon({ name, className }: IconProps) {
  return (
    <svg className={className ? `i ${className}` : 'i'} aria-hidden="true">
      <use href={`#${name}`} />
    </svg>
  );
}
