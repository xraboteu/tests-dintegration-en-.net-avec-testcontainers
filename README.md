# Tests d'intégration avec Testcontainers en .NET

Sample minimal démontrant l'utilisation de Testcontainers pour les tests d'intégration avec ASP.NET Core, EF Core et PostgreSQL.

## Prérequis

- .NET 8.0 SDK
- Docker Desktop (ou Docker Engine) en cours d'exécution

## Structure du projet

- `src/TodoApi` : Application ASP.NET Core Minimal API avec EF Core et PostgreSQL
- `tests/TodoApi.Tests` : Projet de tests xUnit utilisant Testcontainers

## Commandes pour créer le projet

```bash
# Créer la solution
dotnet new sln -n TodoApi

# Créer le projet API
dotnet new web -n TodoApi -o src/TodoApi
dotnet sln add src/TodoApi/TodoApi.csproj

# Créer le projet de tests
dotnet new xunit -n TodoApi.Tests -o tests/TodoApi.Tests
dotnet sln add tests/TodoApi.Tests/TodoApi.Tests.csproj

# Ajouter les packages à l'API
cd src/TodoApi
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
cd ../..

# Ajouter les packages aux tests
cd tests/TodoApi.Tests
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Testcontainers.PostgreSql
dotnet add reference ../../src/TodoApi/TodoApi.csproj
cd ../..
```

## Lancer les tests

```bash
dotnet test
```

## Ce que les tests valident

Les tests d'intégration vérifient :

1. **POST /todos** : Création d'un todo avec retour du statut `Created` et de l'objet créé avec un ID généré
2. **GET /todos/{id}** : Récupération d'un todo existant avec retour du statut `OK` et des données correctes
3. **GET /todos/{id}** (non existant) : Retour du statut `NotFound` pour un ID inexistant

Les tests utilisent une instance PostgreSQL isolée démarrée automatiquement via Testcontainers, garantissant un environnement de test propre et reproductible.

