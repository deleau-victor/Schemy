import { useParams } from 'react-router';

export function EditorPage() {
  const { diagramId } = useParams();
  return (
    <section>
      <h1>Éditeur</h1>
      <p>Schéma {diagramId}</p>
    </section>
  );
}
