import { useQuery } from '@tanstack/react-query';
import { getSystemInfoOptions } from '@/generated/@tanstack/react-query.gen';
import styles from './ApiStatus.module.css';

export function ApiStatus() {
  const { data, isPending, isError } = useQuery(getSystemInfoOptions());

  if (isPending)
    return (
      <span role="status" className={styles.status}>
        Connexion…
      </span>
    );
  if (isError) {
    return (
      <span role="status" className={`${styles.status} ${styles.offline}`}>
        API hors ligne
      </span>
    );
  }
  return (
    <span role="status" className={styles.status}>
      API {data.data.version}
    </span>
  );
}
