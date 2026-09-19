<p align="center">
  <a href="README.md">🇬🇧 English</a> · <a href="README.fr.md">🇫🇷 Français</a>
</p>

# Job Hunter

> Un système d'aide à la décision pour la recherche d'emploi — pas un simple web scraper.

Job Hunter découvre, collecte, normalise, évalue et suit les offres d'emploi à partir des APIs officielles des systèmes ATS (Applicant Tracking System), pour vous aider à vous concentrer sur les postes qui correspondent réellement à votre profil.

Construit avec **.NET 10 / ASP.NET Core** comme un projet backend réaliste démontrant des pratiques de production professionnelles.

---

## Le Problème

La recherche d'emploi est cassée :

- **Sources fragmentées** — chaque entreprise utilise un portail carrière différent.
- **Bruit** — défiler chaque jour à travers des centaines d'offres Senior ou sans rapport.
- **Aucune mémoire** — oublier où on a postulé, quelle version du CV on a utilisée, et ce qui a fonctionné.
- **Scrapers génériques** — des sélecteurs CSS fragiles, des doublons partout, et zéro intelligence.
- **Bots d'auto-candidature** — des candidatures spam qui font chuter le taux de réponse.

## La Solution

Job Hunter adopte une approche **qualité plutôt que quantité** :

```
Découvrir → Collecter → Normaliser → Dédupliquer → Filtrer → Évaluer → Analyser → Réviser → Personnaliser → Suivre → Apprendre
```

Au lieu de scraper chaque site individuellement, il s'intègre directement avec les **plateformes ATS** qui alimentent des milliers de pages carrière — un seul adaptateur couvre des centaines d'entreprises.

---

## Fonctionnalités Clés

| Fonctionnalité | Description |
|----------------|-------------|
| **Ingestion ATS-First** | Intégrations directes avec les APIs Greenhouse, Lever, SmartRecruiters et SAP SuccessFactors |
| **Découverte d'Entreprises** | Détecte automatiquement quel ATS une entreprise utilise à partir de sa page carrière |
| **Schéma Unifié** | Chaque offre — quelle que soit la source — est normalisée en un modèle domaine cohérent |
| **Déduplication Intelligente** | Empêche la même offre d'apparaître deux fois à travers différentes sources |
| **Filtres Stricts** | Élimine les offres non pertinentes (mauvaise séniorité, localisation, langue) avant tout scoring |
| **Scoring Déterministe** | Moteur de scoring pondéré (adéquation du rôle, compétences, expérience, localisation, langue, contrat, fraîcheur) |
| **Analyse IA** | Analyse approfondie par LLM — uniquement pour les candidats les mieux classés afin de minimiser les coûts |
| **Personnalisation du CV** | Réorganiser, reformuler et mettre en avant — jamais inventer. Une couche de validation garantit l'honnêteté |
| **Suivi des Candidatures** | Suivi complet du cycle de vie, de la découverte à l'offre/refus |
| **Analytics** | Taux de réponse, taux d'entretien et métriques de conversion ventilés par rôle, source, version du CV |
| **Moteur d'Apprentissage** | Ajuste les poids du scoring en fonction des résultats réels des candidatures |

---

## Architecture

Job Hunter suit une architecture **Monolithe Modulaire** avec une séparation claire des responsabilités :

```
┌─────────────────────────────┐
│          Jobtri.Api         │   API REST / OpenAPI
│        ASP.NET Core         │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│      Jobtri.Application     │   Cas d'utilisation / Orchestration
│   Services / Interfaces     │
└──────────────┬──────────────┘
               │
        ┌───────┼───────┐
        │       │       │
        ▼       ▼       ▼
   Découverte Scoring   Suivi
        │
        ▼
   Détection ATS
        │
   ┌────┼─────┬──────────┐
   │    │     │          │
   ▼    ▼     ▼          ▼
  GH  Lever  SR    SuccessFactors
   │    │     │          │
   └────┴─────┴──────────┘
        │
        ▼
   Normalisation → Déduplication → EF Core → SQLite
```

### Structure du Projet

```
job-hunter/
│
├── src/
│   ├── Jobtri.Api/                   # API REST, OpenAPI, health checks
│   ├── Jobtri.Application/           # Cas d'utilisation, interfaces, scoring, normalisation
│   ├── Jobtri.Domain/                # Entités, value objects, enums, règles métier
│   └── Jobtri.Infrastructure/        # EF Core, clients ATS, intégrations externes
│
├── tests/
│   ├── Jobtri.UnitTests/
│   └── Jobtri.IntegrationTests/
│
├── docs/
│   ├── ARCHITECTURE.md               # Blueprint architectural complet (41 sections)
│   ├── PROJECT_DECISIONS.md           # Registre des Décisions Architecturales (D001–D024)
│   └── ROADMAP.md                     # Feuille de route (28 phases)
│
└── README.md
```

### Principes de Conception

- **Le Domaine n'a aucune dépendance externe** — pas d'EF Core, pas de HttpClient, aucune connaissance des ATS
- **La couche Application définit les contrats** — l'Infrastructure les implémente
- **Les adaptateurs de source ne font que récupérer les données** — toute la logique métier reste dans .NET
- **Isolation des pannes** — la panne d'un ATS ne bloque jamais les autres
- **IA indépendante du fournisseur** — le fournisseur LLM est interchangeable derrière une interface

---

## Stack Technique

| Couche | Technologie |
|--------|-------------|
| Langage | C# |
| Runtime | .NET 10 LTS |
| Framework Web | ASP.NET Core |
| ORM | Entity Framework Core |
| Base de données (MVP) | SQLite |
| Base de données (Production) | PostgreSQL |
| Clients HTTP | `IHttpClientFactory` + Typed Clients |
| Résilience | Politiques de résilience .NET (timeout, retry, gestion du rate-limit) |
| Logging | Microsoft.Extensions.Logging / Serilog |
| Tests | xUnit |
| Documentation API | OpenAPI |
| Conteneurisation | Docker |
| CI/CD | GitHub Actions |

---

## Pipeline de Traitement

```
Registre d'Entreprises
       ↓
  Détection ATS          ← Greenhouse / Lever / SmartRecruiters / SuccessFactors
       ↓
  Acquisition des Offres ← APIs ATS publiques → JSON-LD → HTTP direct
       ↓
  Normalisation          ← Titre, localisation, séniorité, contrat, langue
       ↓
  Déduplication          ← Empreinte par identifiant canonique
       ↓
  Filtres Stricts        ← Pays, séniorité, expérience, langue, exclusions
       ↓
  Scoring Déterministe   ← Score composite pondéré 0–100
       ↓
  Analyse IA             ← Uniquement pour les meilleurs candidats (score ≥ seuil)
       ↓
  Revue Humaine          ← C'est vous qui décidez où postuler
       ↓
  CV / LM Personnalisés  ← Réorganiser + reformuler + mettre en avant (jamais inventer)
       ↓
  Suivi des Candidatures ← Cycle complet : Postulé → Entretien → Offre/Refus
       ↓
  Analytics              ← Taux de réponse, taux d'entretien, conversion par segment
       ↓
  Moteur d'Apprentissage ← Ajustement des poids basé sur les résultats
```

---

## Moteur de Scoring

Chaque offre reçoit un score déterministe avant toute intervention de l'IA :

| Critère | Poids | Ce qu'il mesure |
|---------|-------|-----------------|
| Adéquation du Rôle | 30 | Proximité du titre avec les rôles ciblés |
| Compétences | 25 | Chevauchement entre compétences requises et possédées |
| Expérience | 15 | Adéquation de la plage d'expérience |
| Localisation | 10 | Correspondance géographique et compatibilité remote |
| Langue | 10 | Langues requises vs. parlées |
| Contrat | 5 | Alignement CDI, CDD, freelance |
| Fraîcheur | 5 | Ancienneté de la publication de l'offre |
| **Total** | **100** | |

---

## Cycle de Vie des Candidatures

```
Nouvelle → Présélectionnée → Prête → Postulée → Relance
                                        ↓
                    Répondue → Pré-entretien → Entretien → Entretien Technique → Entretien Final
                                                                                        ↓
                                                                                  Offre / Refus
```

Autres états terminaux : `Retirée`, `Ignorée`

---

## Points d'Accès API

```http
GET    /api/jobs                        # Lister les offres (filtrable, paginé)
GET    /api/jobs/{id}                   # Détails d'une offre
GET    /api/jobs/top                    # Offres les mieux classées
POST   /api/jobs/{id}/shortlist         # Présélectionner une offre

GET    /api/companies                   # Lister les entreprises
POST   /api/companies                   # Ajouter une entreprise
POST   /api/companies/{id}/discover     # Lancer la découverte ATS

GET    /api/applications                # Lister les candidatures
POST   /api/applications                # Créer une candidature
PATCH  /api/applications/{id}/status    # Mettre à jour le statut

GET    /api/analytics/summary           # Tableau de bord analytique
```

---

## Démarrage Rapide

### Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Build & Exécution

```bash
# Cloner le dépôt
git clone https://github.com/your-username/job-hunter.git
cd job-hunter

# Build
dotnet build

# Lancer les tests
dotnet test

# Lancer l'API
dotnet run --project src/Jobtri.Api
```

L'API sera disponible sur `https://localhost:5001` avec Swagger UI sur `/swagger`.

---

## Feuille de Route

Le projet est construit de manière incrémentale à travers 28 phases. Voir [ROADMAP.md](docs/ROADMAP.md) pour le plan complet.

### Focus Actuel : MVP (Phases 0–14)

- [x] Architecture et documentation des décisions
- [ ] Fondation du projet et structure de la solution
- [ ] Entités du domaine et contrats
- [ ] Persistance EF Core + SQLite
- [ ] Intégration Greenhouse
- [ ] Intégration Lever
- [ ] Intégration SmartRecruiters
- [ ] Couche de normalisation
- [ ] Moteur de déduplication
- [ ] Découverte d'entreprises et détection ATS
- [ ] Intégration SAP SuccessFactors
- [ ] Configuration du profil maître
- [ ] Filtres stricts
- [ ] Scoring déterministe
- [ ] Premier résultat de recherche d'emploi réel

### Post-MVP

- [ ] API REST avec OpenAPI
- [ ] Découverte d'offres en arrière-plan
- [ ] Logging structuré et résilience
- [ ] Suivi des candidatures
- [ ] Analyse IA et personnalisation du CV
- [ ] Analytics et moteur d'apprentissage
- [ ] Migration PostgreSQL
- [ ] Docker et CI/CD

---

## Métrique de Succès

L'objectif principal n'est **pas** le nombre d'offres récupérées.

```
North Star  =  Entretiens / Candidatures Qualifiées
```

Métriques secondaires : taux de réponse, taux d'entretien, taux d'offre — segmentés par rôle, localisation, source, ATS et version du CV.

---

## Décisions de Conception

Toutes les décisions architecturales majeures sont formellement enregistrées dans [PROJECT_DECISIONS.md](docs/PROJECT_DECISIONS.md). Points clés :

| Décision | Choix | Justification |
|----------|-------|---------------|
| D003 | Architecture ATS-first | Un seul adaptateur couvre des centaines d'entreprises |
| D005 | .NET 10 comme stack principal | Valeur portfolio pour les postes Backend .NET |
| D010 | Filtres stricts avant l'IA | Réduire les coûts, la latence et les erreurs |
| D013 | Humain dans la boucle | Pas d'auto-candidature massive — qualité plutôt que quantité |
| D014 | Pas de fabrication dans les CV | Réorganiser et reformuler uniquement — jamais inventer |
| D017 | Petit MVP d'abord | Livrer de la valeur avant d'élargir le périmètre |
| D019 | Monolithe Modulaire | Architecture propre sans la surcharge des microservices |

---

## Documentation

| Document | Description |
|----------|-------------|
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Blueprint architectural complet — 41 sections couvrant chaque couche |
| [PROJECT_DECISIONS.md](docs/PROJECT_DECISIONS.md) | 24 Registres de Décisions Architecturales formels |
| [ROADMAP.md](docs/ROADMAP.md) | Plan d'implémentation en 28 phases avec Definition of Done par phase |

---

## Licence

Ce projet est destiné à un usage personnel et à la démonstration de compétences en portfolio.

---

<p align="center">
  <i>Conçu pour trouver le bon emploi — pas n'importe quel emploi.</i>
</p>
