import { Link } from 'react-router';

export function NotFoundPage() {
  return (
    <section>
      <h1>Page introuvable</h1>
      <Link to="/">Retour à Mes schémas</Link>
    </section>
  );
}
