import { useParams } from 'react-router';

export type Place = 'folder' | 'recent' | 'starred' | 'trash';

export const PLACE_LABELS: Record<Place, string> = {
  folder: 'Mes schémas',
  recent: 'Récents',
  starred: 'Favoris',
  trash: 'Corbeille',
};

export function ExplorerPage({ place }: { place: Place }) {
  const { folderId } = useParams();
  return (
    <section>
      <h1>{PLACE_LABELS[place]}</h1>
      {place === 'folder' && folderId ? <p>Dossier {folderId}</p> : null}
    </section>
  );
}
