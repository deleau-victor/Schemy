# Setup de l'environnement de développement

Ce document liste le nécessaire pour pouvoir contribuer à ce projet.
Les commandes sont données pour un shell POSIX (bash/zsh) et, quand elles diffèrent, pour PowerShell 7.

## 1. Outils

| Outil                 | Version    | Vérification                           |
| --------------------- | ---------- | -------------------------------------- |
| .NET SDK              | ≥ 10.0.100 | `dotnet --version`                     |
| Aspire CLI            | ≥ 13.2     | `aspire --version`                     |
| Runtime de conteneurs | voir § 2   | `docker version` ou `podman --version` |
| Vite+ (`vp`)          | ≥ 1.2.0    | `vp --version` (`vp upgrade` sinon)    |

- `vp` doit être dans le `PATH` du shell qui lance `aspire run` : Aspire démarre le front avec `vp dev`.
- pnpm n'a pas besoin d'être installé : `vp` télécharge la version déclarée dans `src/Web/package.json`.

## 2. Runtime de conteneurs

Aspire et Testcontainers ont besoin d'un runtime compatible Docker. Deux options :

### Option A : Docker (le plus simple)

Docker Desktop (Windows/macOS) ou Docker Engine (Linux). Rien d'autre à configurer : Aspire et Testcontainers le détectent tout seuls.

### Option B : Podman (≥ 5)

Variables communes à définir, quel que soit l'OS :

| Variable                       | Valeur   | utilité                                      |
| ------------------------------ | -------- | -------------------------------------------- |
| `ASPIRE_CONTAINER_RUNTIME`     | `podman` | force Aspire à utiliser Podman               |
| `DOCKER_HOST`                  | voir OS  | permet à Testcontainers de trouver le socket |
| `TESTCONTAINERS_RYUK_DISABLED` | `true`   | Ryuk ne fonctionne pas avec Podman rootless  |

### Linux (Podman rootless)

```bash
systemctl --user enable --now podman.socket
```

Dans le fichier de démarrage de votre shell (`~/.bashrc`, `~/.zshrc`, …) :

```bash
export ASPIRE_CONTAINER_RUNTIME=podman
export DOCKER_HOST="unix://$XDG_RUNTIME_DIR/podman/podman.sock"
export TESTCONTAINERS_RYUK_DISABLED=true
```

### macOS / Windows (Podman machine)

Podman tourne dans une VM :

```bash
podman machine init    # une seule fois
podman machine start
```

Récupérer le chemin du socket à mettre dans DOCKER_HOST :

```bash
podman machine inspect --format '{{.ConnectionInfo.PodmanSocket.Path}}'
```

- macOS : `DOCKER_HOST="unix://<chemin>"`, à exporter dans `~/.zshrc`.
- Windows : `DOCKER_HOST` vaut le named pipe affiché (`npipe:////./pipe/...`). Pour définir les variables de façon persistante :

```ps1
[Environment]::SetEnvironmentVariable("ASPIRE_CONTAINER_RUNTIME", "podman", "User")
[Environment]::SetEnvironmentVariable("DOCKER_HOST", "npipe:////./pipe/podman-machine-default", "User")
[Environment]::SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true", "User")
```

Rouvrir le terminal (et l'IDE) ensuite.
Si Podman Desktop a activé la compatibilité Docker, `DOCKER_HOST` est souvent inutile.

### Image MinIO (Podman uniquement)

Aspire + Podman ne tire pas l'image MinIO tout seul. À faire une fois :

```bash
podman pull docker.io/pgsty/minio:RELEASE.2026-08-04T00-00-00Z
```

(Les images officielles `minio/minio` ont été retirées ; `pgsty/minio` est une reconstruction communautaire.)

## Secrets (jamais versionnés)

Deux mots de passe aléatoires à stocker dans les user-secrets de l'AppHost.

bash / zsh :

```bash
dotnet user-secrets set "Parameters:postgres-password" "$(openssl rand -base64 24)" --project src/AspireHost
dotnet user-secrets set "Parameters:minio-password"    "$(openssl rand -base64 24)" --project src/AspireHost
```

PowerShell :

```ps1
function New-Secret { [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(24)) }
dotnet user-secrets set "Parameters:postgres-password" (New-Secret) --project src/AspireHost
dotnet user-secrets set "Parameters:minio-password"    (New-Secret) --project src/AspireHost
```

## Ports de dev

| Ressource           | Port                        |
| ------------------- | --------------------------- |
| Postgres            | 5433                        |
| API                 | 7010 (Swagger : `/swagger`) |
| Web                 | 5180                        |
| MinIO API / console | 9010 / 9011                 |

Ces ports doivent être libres sur la machine.

## Lancer

Depuis la racine du dépôt :

```bash
aspire run
```

## Vérifier

```bash
cd src/Web
vp run ci
```
