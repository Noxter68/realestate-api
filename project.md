# RealEstate SaaS Academy — Formation complète (Local MVP)

## .NET 8 + EF Core Migrations + LINQ + Angular + Tailwind + PostgreSQL + pgAdmin

> **But** : apprendre **vraiment** C#/.NET 8 + **EF Core** + **LINQ** en construisant un MVP immobilier **ultra clean**.  
> **Résultat** : une plateforme locale démo-ready avec :
>
> - **Back-office Agence** (CRUD biens, leads, visites, notes)
> - **Site public** (listing + détail + formulaire de contact → lead agence)
> - **DB PostgreSQL** visible dans **pgAdmin**
> - **Migrations EF Core** (versioning DB pro)
> - **Frontend Angular + Tailwind** (UI clean, rapide)

---

# 0) Règles de la formation (IMPORTANT)

1. On suit ce fichier **dans l’ordre**.
2. On avance **étape par étape** :
   - Tu fais les actions
   - Tu testes
   - Tu valides que ça marche
   - **On ne passe pas à l’étape suivante sans validation**
3. À la fin de chaque chapitre :
   - ✅ Checklist de vérification
   - 🧠 Mini exercices LINQ (quand applicable)
4. On vise le **MVP local** : pas de cloud, pas de déploiement.

---

# 1) Pré-requis (logiciels à installer)

> Si tu as déjà certains outils, tu peux sauter l’installation, mais vérifie les versions.

## 1.1 Outils obligatoires

- **Git** (pour versionner)
- **.NET SDK 8**
- **Node.js LTS** (pour Angular)
- **Angular CLI**
- **Docker Desktop** (pour PostgreSQL + pgAdmin en local)
- **pgAdmin** (via Docker, donc pas besoin d’installer à part)
- **Un éditeur** : VS Code (recommandé) ou JetBrains Rider

## 1.2 Vérifier que tout est installé (commandes)

Exécute ces commandes dans ton terminal :

```bash
git --version
dotnet --version
node --version
npm --version
ng version
docker --version
docker compose version


✅ Attendu :

dotnet retourne 8.x

node retourne une version LTS

ng version fonctionne

docker compose fonctionne

✅ CHECKPOINT #0

 Tous les outils répondent

 Tu peux lancer Docker Desktop sans erreur

2) Vision du MVP (ce qui doit fonctionner)
2.1 Back-office Agence (protégé par login)

Register agence (crée org + owner)

Login

Dashboard

CRUD Properties (biens)

Gestion Leads (demandes de contact)

Gestion Visits (visites)

Internal Notes (notes internes)

2.2 Site public (sans login)

Page agence : /a/:slug

Liste des biens Published

Page détail : /p/:id

Formulaire contact → crée un Lead en base, visible côté agence

3) Structure du projet (2 apps séparées)

On fait un repo simple (pas monorepo compliqué) :

realestate-saas/
  infra/
    docker-compose.yml
  api/
    Api/ (projet .NET)
  web/
    (projet Angular)
  docs/
    project.md

4) Infra locale (Postgres + pgAdmin) — CHAPITRE 1
Objectifs

Avoir une DB PostgreSQL locale + pgAdmin

Créer la DB realestate_mvp

Étapes
1. Créer le dossier infra
mkdir -p realestate-saas/infra
cd realestate-saas

2. Créer infra/docker-compose.yml

Copie-colle tel quel :

services:
  postgres:
    image: postgres:16
    container_name: realestate_postgres
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: realestate_mvp
    ports:
      - "5432:5432"
    volumes:
      - realestate_pgdata:/var/lib/postgresql/data

  pgadmin:
    image: dpage/pgadmin4:8
    container_name: realestate_pgadmin
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@local.dev
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "5050:80"
    depends_on:
      - postgres

volumes:
  realestate_pgdata:

3. Lancer les containers
cd infra
docker compose up -d

4. Ouvrir pgAdmin

Ouvre http://localhost:5050

Login :

Email : admin@local.dev

Password : admin

5. Ajouter un serveur dans pgAdmin

Add New Server

Name : RealEstate Local

Connection:

Host name/address : postgres (si tu es dans réseau docker)
OU localhost (souvent le plus simple)

Port : 5432

Username : postgres

Password : postgres

✅ La DB realestate_mvp doit exister.

Vérifications (ne pas passer si ça bloque)

pgAdmin accessible

DB visible

Connection OK

✅ CHECKPOINT #1

 docker compose ps montre postgres + pgadmin “Up”

 pgAdmin accessible sur localhost:5050

 Connexion à la DB OK

 realestate_mvp visible

5) Backend .NET 8 (Web API Clean + Swagger) — CHAPITRE 2
Objectifs

Créer une API .NET 8 clean

Swagger OK

Endpoint /health

Étapes
1. Créer le projet API

Depuis la racine :

cd ..
mkdir -p api
cd api
dotnet new webapi -n Api
cd Api

2. Lancer l’API (test brut)
dotnet run


Ouvre l’URL affichée (Swagger normalement /swagger)

3. Nettoyer le template

Supprimer WeatherForecast.cs

Supprimer le controller WeatherForecast

4. Ajouter un Health endpoint

Créer Controllers/HealthController.cs :

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });
}

5. CORS (pour Angular plus tard)

Dans Program.cs, ajoute une policy CORS “dev” (simple) :

Autoriser http://localhost:4200

✅ CHECKPOINT #2

 GET /health retourne { "status": "ok" }

 Swagger accessible

6) EF Core + PostgreSQL + MIGRATIONS — CHAPITRE 3
Objectifs

Installer EF Core

Configurer DbContext

Générer migration initiale

Appliquer migration → tables visibles pgAdmin

Comprendre __EFMigrationsHistory

Étapes
1. Installer les packages EF Core
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

2. Créer les dossiers

Dans api/Api :

Domain/Entities
Infrastructure/Data

3. Créer entités Core

Créer Domain/Entities/Organization.cs, User.cs, RefreshToken.cs (on fera simple au début).

4. Créer AppDbContext

Créer Infrastructure/Data/AppDbContext.cs :

DbSet Organizations, Users, RefreshTokens

Config index sur OrganizationId

Conventions de table (snake_case optionnel plus tard)

5. Ajouter connection string

Dans appsettings.Development.json :

Host=localhost, Port=5432, Database=realestate_mvp, Username=postgres, Password=postgres

6. Déclarer DbContext dans Program.cs

builder.Services.AddDbContext<AppDbContext>(...)

7. Installer l’outil dotnet-ef (si nécessaire)
dotnet tool install --global dotnet-ef

8. Créer la migration initiale
dotnet ef migrations add InitialCreate

9. Appliquer la migration
dotnet ef database update

✅ CHECKPOINT #3

 Migration créée dans /Migrations

 database update passe sans erreur

 Tables visibles dans pgAdmin

 Table __EFMigrationsHistory présente

7) Auth JWT + Refresh Token — CHAPITRE 4
Objectifs

Register agence (Organization + User owner)

Login

JWT avec claims : userId, organizationId, role

Refresh token stocké en DB (hash)

Endpoint /me

Étapes (haute précision, à suivre)

Créer DTOs : RegisterAgencyRequest, LoginRequest, AuthResponse

Créer AuthService :

Hash password (PBKDF2 ou équivalent .NET)

Verify password

JWT generation

Refresh token :

Random token

Stocker hash + expiry + user_id

AuthController :

POST /auth/register-agency

POST /auth/login

POST /auth/refresh

POST /auth/logout

Ajouter [Authorize] + config Auth middleware

Créer GET /me (protégé)

✅ CHECKPOINT #4

 Register agence retourne tokens

 Login retourne tokens

 /me retourne user + org

 Refresh fonctionne

 Refresh token présent en DB

8) Multi-tenant + LINQ base (foundation SaaS) — CHAPITRE 5
Objectifs

Toutes les données agence isolées

LINQ propre : .Select vers DTO, filtres, pagination

Préparer le terrain pour Properties/Leads

Étapes

Ajouter OrganizationId aux futures entités métier

Mettre en place un CurrentUser service :

Lit HttpContext.User claims

Implémenter filtrage tenant :

Option A (simple & clean) : filtrer dans Services (Where(x => x.OrganizationId == currentOrgId))

Option B (avancé) : global query filter (plus tard si besoin)

Créer helper pagination :

PageResult<T> + ToPagedResultAsync()

Écrire 3 endpoints test LINQ (ex: list users paginés, search email)

Exercices LINQ

Endpoint GET /users/search?q= :

case-insensitive

tri createdAt desc

projection DTO (pas entity)

✅ CHECKPOINT #5

 2 agences différentes ne voient pas les mêmes données

 Pagination OK

 Search OK

9) Domaine Immobilier (Properties, Leads, Visits, Notes) — CHAPITRE 6
Objectifs

Ajouter les entités métier

Apprendre migrations sur feature

Comprendre relations (Property → Visits, Property → Leads)

Étapes

Créer entities :

Property, Lead, Visit, InternalNote, Contact (optionnel MVP)

Ajouter DbSets

Config relations (FK)

Créer migration :

dotnet ef migrations add AddRealEstateDomain

Appliquer :

dotnet ef database update

✅ CHECKPOINT #6

 Tables properties/leads/visits/internal_notes créées en DB

 FK property_id OK

10) API Back-office Agence — CHAPITRE 7
Objectifs

CRUD Properties + list avec filtres

Leads list + update status

Visits create/list

Internal notes create/list

Étapes
1) Properties

GET /agency/properties?status=&q=&page=&pageSize=&sort=

POST /agency/properties

GET /agency/properties/:id

PUT /agency/properties/:id

DELETE /agency/properties/:id

LINQ obligatoire :

q recherche title/city

status filter

sort (price, createdAt)

pagination

2) Leads

GET /agency/leads?status=&q=&page=&pageSize=

PUT /agency/leads/:id/status

LINQ :

search q sur name/email/message

status filter

order by createdAt desc

3) Visits

GET /agency/properties/:id/visits

POST /agency/properties/:id/visits

4) Notes

POST /agency/notes

GET /agency/notes?entity_type=&entity_id=

✅ CHECKPOINT #7

 Tout fonctionne via Swagger

 Création property OK

 Listing filtré/paginé OK

 Leads OK

 Visits OK

 Notes OK

11) Angular + Tailwind setup — CHAPITRE 8
Objectifs

Créer le front Angular

Installer Tailwind

Pages auth + layout dashboard

Connexion API login

Étapes
1. Créer Angular

Depuis la racine :

cd ../../
ng new web --standalone --routing --style=css
cd web

2. Installer Tailwind
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init

3. Config Tailwind

tailwind.config.js :

module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: { extend: {} },
  plugins: [],
}

4. Ajouter Tailwind dans src/styles.css
@tailwind base;
@tailwind components;
@tailwind utilities;

5. Test Tailwind

Mettre un titre stylé dans app.component.html :

si c’est stylé → OK

6. Auth UI

routes : /register, /login, /dashboard

AuthService

stockage tokens localStorage

Interceptor Bearer

Guard routes

✅ CHECKPOINT #8

 Tailwind fonctionne

 Register/Login marche

 Dashboard protégé marche

12) Angular Back-office (Agence) — CHAPITRE 9
Objectifs

UI Properties list + create/edit

UI Leads list + update status

UI Visits sur property detail

UI Notes

Étapes

Layout dashboard Tailwind (sidebar + topbar)

Pages :

/agency/properties

/agency/properties/new

/agency/properties/:id

/agency/leads

Components réutilisables :

Table

Button

Input

Connexion aux endpoints API

Gestion erreurs (message simple)

✅ CHECKPOINT #9

 CRUD properties depuis le UI

 Leads visibles + statut modifiable

13) Site public (Particuliers) — API — CHAPITRE 10
Objectifs

Public listing par agence slug

Détail property

Formulaire contact → crée Lead

Étapes

GET /public/agencies/:slug/properties

retourne uniquement Published

filtres minPrice/maxPrice/city/type

GET /public/properties/:id

POST /public/properties/:id/lead

crée lead (OrganizationId de l’agence)

status = New

✅ CHECKPOINT #10

 Listing public marche via Swagger

 Post lead crée une ligne en DB et visible côté agence

14) Site public (Particuliers) — Angular — CHAPITRE 11
Objectifs

Pages publiques Angular

Contact form

Démo end-to-end

Étapes

Routes :

/a/:slug (listing)

/p/:id (detail)

Listing : cards Tailwind

Detail : infos + form contact

Form submit → API lead

Message succès

✅ CHECKPOINT #11 (FIN MVP)

 Je vois les annonces publiques

 J’envoie une demande

 Je vois le lead côté agence

15) Démo finale (script)

Register agence “ImmoParis” (slug immoparis)

Créer 3 properties (2 Published, 1 Draft)

Aller sur /a/immoparis → voir 2 annonces

Ouvrir détail → envoyer lead

Retour /agency/leads → lead visible → status “Contacted”
```
