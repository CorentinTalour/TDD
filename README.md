# TDD Project

## Description

Ce projet implémente des tests unitaires en utilisant .NET et suit les principes du développement piloté par les tests (TDD). Il contient une API, ainsi que des tests unitaires pour valider le bon fonctionnement des fonctionnalités du projet. L'objectif de ce projet est de démontrer une bonne utilisation des tests unitaires et de la gestion des erreurs dans une API .NET.

Ce projet utilise GitHub Actions pour l'intégration continue (CI), permettant d'exécuter les tests unitaires à chaque push sur la branche `main`.

## Table des matières

1. [Technologies utilisées](#technologies-utilisées)
2. [Prérequis](#prérequis)
3. [Installation](#installation)
4. [Exécution des tests](#exécution-des-tests)

---

## Technologies utilisées

- **.NET SDK 8.0** - Framework pour le développement d'applications API et web.
- **xUnit / MSTest** - Frameworks de tests unitaires utilisés pour effectuer les tests.
- **GitHub Actions** - CI/CD pour l'exécution des tests à chaque push sur la branche `main`.
- **PostgreSQL** - Base de données utilisée pour la persistance des données (si applicable).
- **Entity Framework Core** - ORM pour l'interaction avec la base de données.
- **Docker** (facultatif) - Pour les configurations de conteneur si nécessaire.

---

## Prérequis

Avant de commencer, assurez-vous d'avoir installé les outils suivants sur votre machine :

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) - Pour compiler et exécuter les projets .NET.
- Un éditeur de texte comme [Visual Studio](https://visualstudio.microsoft.com/), [VSCode](https://code.visualstudio.com/) ou [Rider](https://www.jetbrains.com/rider/).
---

## Installation

### Cloner le projet

Clonez ce dépôt sur votre machine locale :

```bash
git clone https://github.com/votre-utilisateur/TDD.git
cd TDD
```

### Restaurer les dépendances

Avant de démarrer l’application, vous devez restaurer les dépendances des projets .NET dans la solution :

```bash
dotnet restore
```

### Construire le projet

Construisez le projet pour vérifier que tout est bien configuré :

```bash
dotnet build --configuration Release
```

---

## Exécution des tests

### Exécution des tests localement

Pour exécuter les tests unitaires du projet localement, utilisez la commande suivante :

```bash
dotnet test ./TestTDD/TestTDD.csproj --configuration Release
```

Cette commande exécutera tous les tests unitaires définis dans le projet de tests.

### Exécution des tests avec GitHub Actions

Les tests sont également configurés pour s’exécuter automatiquement sur GitHub à chaque push sur la branche main. GitHub Actions exécutera les tests et vous donnera un rapport sur la 
réussite ou l’échec des tests dans l’interface de votre dépôt GitHub.
