# NDGF

> NoDepthGoodFriends — application de gestion de dépenses partagées entre groupes d'utilisateurs (façon "Tricount").

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/licence-non%20définie-lightgrey)](#licence)

## Sommaire

- [Présentation](#présentation)
- [Fonctionnalités](#fonctionnalités)
- [Architecture](#architecture)
- [Stack technique](#stack-technique)
- [Structure du projet](#structure-du-projet)
- [Prérequis](#prérequis)
- [Installation](#installation)
  - [Avec Docker (recommandé)](#avec-docker-recommandé)
  - [En local (développement)](#en-local-développement)
- [Configuration](#configuration)
- [Utilisation](#utilisation)
- [Tests](#tests)
- [Documentation de l'API](#documentation-de-lapi)
- [Contribuer](#contribuer)
- [Licence](#licence)
- [Contact](#contact)

## Présentation

**NDGF** est une application web permettant à des groupes d'utilisateurs de gérer et de répartir équitablement des dépenses communes. Chaque membre d'un groupe peut ajouter des dépenses, consulter l'historique des mouvements du groupe, et le système calcule automatiquement les remboursements nécessaires pour équilibrer les comptes entre les participants.

## Fonctionnalités

- 🔐 Authentification et gestion des utilisateurs sécurisée par JWT (avec refresh token)
- 👥 Création et gestion de groupes d'utilisateurs
- 💸 Ajout et répartition de dépenses (`Expense` / `ExpensePart`) entre les membres d'un groupe
- 📊 Calcul automatique des soldes et des remboursements (`Refund`) à effectuer entre les membres
- 🕒 Historique des événements d'un groupe (`GroupHistory`)
- 📄 Documentation interactive de l'API via Scalar / OpenAPI

## Architecture

Le projet suit les principes de la **Clean Architecture**, organisée en couches indépendantes afin de séparer les responsabilités et de faciliter la maintenance et les tests :

```
ndgf.Domain          → Entités métier, règles de domaine, exceptions
ndgf.Application      → Cas d'usage (Commands/Queries), interfaces, DTO métier
ndgf.Infrastructure    → Accès aux données (EF Core), sécurité, migrations
ndgf.Api             → Endpoints HTTP (Minimal API), exposition de l'API REST
ndgf.Web             → Application Blazor (serveur + WebAssembly), interface utilisateur
```

Le flux de dépendances va toujours vers l'intérieur (`Api`/`Web` → `Infrastructure` → `Application` → `Domain`), conformément au principe d'inversion des dépendances.

## Stack technique

| Domaine | Technologie |
|---|---|
| Langage / Runtime | C# / .NET 10 |
| API | ASP.NET Core Minimal API |
| Interface web | Blazor (Server + WebAssembly) |
| Accès aux données | Entity Framework Core |
| Base de données | Microsoft SQL Server 2022 |
| Authentification | JWT Bearer (access + refresh token) |
| Documentation API | Scalar / OpenAPI |
| Conteneurisation | Docker / Docker Compose |

## Structure du projet

```
ndgf/
├── ndgf.Api/                  # API REST (endpoints, DTOs, mappers)
├── ndgf.Application/           # Logique applicative (commands, queries, handlers, services)
├── ndgf.Application.Tests/      # Tests unitaires de la couche application
├── ndgf.Domain/                # Entités et logique métier
├── ndgf.Domain.Tests/           # Tests unitaires de la couche domaine
├── ndgf.Infrastructure/         # Persistance (EF Core), migrations, sécurité
├── ndgf.Web/
│   ├── ndgf.Web/               # Hôte Blazor (Server)
│   └── ndgf.Web.Client/         # Client Blazor WebAssembly
├── docker/
│   ├── api/Dockerfile
│   └── web/Dockerfile
├── compose.yaml                # Orchestration Docker (db, api, web)
└── ndgf.sln                    # Solution .NET
```

## Prérequis

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started) et Docker Compose (pour le lancement conteneurisé)
- [SQL Server](https://www.microsoft.com/sql-server) (local ou via Docker)
- Un IDE compatible .NET (JetBrains Rider, Visual Studio, VS Code…)

## Installation

Clonez le dépôt :

```bash
git clone https://github.com/Peemers/ndgf.git
cd ndgf
```

### Avec Docker (recommandé)

Le fichier `compose.yaml` fournit une orchestration complète : base de données SQL Server, API et application web.

1. Créez un fichier `.env` à la racine du projet en vous basant sur les variables suivantes :

```env
DB_PASSWORD=VotreMotDePasseSecurise
CONNECTION_STRING=Server=db;Database=NdgfDb;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=True;
JWT_SECRET=UneCleSecreteSuffisammentLongue
JWT_ISSUER=NdgfApi
JWT_AUDIENCE=NdgfWeb
JWT_EXPIRATION_MINUTES=15
```

2. Lancez les conteneurs :

```bash
docker compose up -d
```

3. Les services sont exposés sur :
   - API : http://localhost:5217
   - Application Web : http://localhost:5087
   - SQL Server : localhost:1433

> ⚠️ Ne commitez jamais votre fichier `.env` contenant de véritables secrets. Pensez à l'ajouter à votre `.gitignore` et à régénérer tout mot de passe/clé ayant pu être exposé.

### En local (développement)

1. Restaurez les dépendances et compilez la solution :

```bash
dotnet restore
dotnet build
```

2. Renseignez la configuration (chaîne de connexion, paramètres JWT) dans `ndgf.Api/appsettings.Development.json` ou via les [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) :

```bash
cd ndgf.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=NdgfDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

3. Appliquez les migrations de base de données (appliquées automatiquement au démarrage de l'API, ou manuellement) :

```bash
dotnet ef database update --project ndgf.Infrastructure --startup-project ndgf.Api
```

4. Lancez l'API :

```bash
dotnet run --project ndgf.Api
```

5. Dans un autre terminal, lancez l'application web :

```bash
dotnet run --project ndgf.Web/ndgf.Web
```

## Configuration

Les principales variables de configuration de l'application :

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | Chaîne de connexion à la base SQL Server |
| `Jwt__Secret` | Clé secrète utilisée pour signer les tokens JWT |
| `Jwt__Issuer` | Émetteur (issuer) des tokens JWT |
| `Jwt__Audience` | Audience attendue des tokens JWT |
| `Jwt__ExpirationMinutes` | Durée de validité (en minutes) du token d'accès |

## Utilisation

Une fois l'API et l'application web démarrées :

1. Créez un compte utilisateur via l'application web ou l'endpoint `POST /api/users`.
2. Connectez-vous pour obtenir un token d'accès (JWT) et un refresh token.
3. Créez un groupe et invitez d'autres membres.
4. Ajoutez des dépenses au sein du groupe ; l'application répartit automatiquement les parts entre les membres concernés.
5. Consultez le solde de chaque membre et les remboursements suggérés pour équilibrer les comptes du groupe.

## Tests

Le projet dispose de tests unitaires pour les couches `Domain` et `Application`.

```bash
dotnet test
```

Pour exécuter les tests d'un projet spécifique :

```bash
dotnet test ndgf.Domain.Tests
dotnet test ndgf.Application.Tests
```

## Documentation de l'API

En environnement de développement, une documentation interactive de l'API (basée sur OpenAPI et [Scalar](https://scalar.com/)) est disponible une fois l'API lancée, à l'adresse `/scalar`.

## Contribuer

Les contributions sont les bienvenues ! Pour proposer une modification :

1. Forkez le dépôt
2. Créez une branche pour votre fonctionnalité (`git checkout -b feature/ma-fonctionnalite`)
3. Committez vos changements (`git commit -m 'feat: ajout de ma fonctionnalité'`)
4. Poussez votre branche (`git push origin feature/ma-fonctionnalite`)
5. Ouvrez une Pull Request

Merci de respecter l'architecture en place (Clean Architecture) et d'accompagner toute nouvelle fonctionnalité de tests unitaires pertinents.

## Licence

Aucune licence n'est actuellement définie pour ce projet. Merci de contacter le mainteneur pour toute question relative à l'utilisation ou à la redistribution du code.

## Contact

Projet maintenu par [Peemers](https://github.com/Peemers).
