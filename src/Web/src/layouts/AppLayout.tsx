import { Outlet } from 'react-router';
import { ThemeToggle } from '@/components/ui/ThemeToggle';
import styles from './AppLayout.module.css';

export function AppLayout() {
  return (
    <div className={styles.shell}>
      <header className={styles.header}>
        <span className={styles.brand}>Schemy</span>
        <ThemeToggle />
      </header>
      <main className={styles.main}>
        <Outlet />
      </main>
    </div>
  );
}
