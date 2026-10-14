import type { RouteObject } from 'react-router';
import { AppLayout } from '@/layouts/AppLayout';
import { EditorPage } from '@/pages/EditorPage';
import { ExplorerPage } from '@/pages/ExplorerPage';
import { NotFoundPage } from '@/pages/NotFoundPage';

// Spec § 6.3 — browser history replaces schemy.html's hand-made hist/fwd stacks.
export const routes: RouteObject[] = [
  {
    element: <AppLayout />,
    children: [
      { index: true, element: <ExplorerPage place="folder" /> },
      { path: 'f/:folderId', element: <ExplorerPage place="folder" /> },
      { path: 'recent', element: <ExplorerPage place="recent" /> },
      { path: 'starred', element: <ExplorerPage place="starred" /> },
      { path: 'trash', element: <ExplorerPage place="trash" /> },
      { path: 'd/:diagramId', element: <EditorPage /> },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
];
