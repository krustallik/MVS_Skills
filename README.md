# MVS Skills

MVS Skills is an academic ASP.NET Core MVC application for creating professional profiles and discovering people by profession and skills. It includes account registration, profile and skill management, profile photos, and user ratings.

## Features

- Search profiles by name, profession, and selected skills.
- Register and sign in with ASP.NET Core Identity.
- Create and edit profile entries with professional details and skills.
- Upload profile photos and choose an avatar.
- Rate profiles with a 1–5 choice; self-ratings and repeat ratings for the same profile are blocked.
- Use admin-only screens to manage accounts and skills.

## Technology

- ASP.NET Core MVC on .NET 8
- Entity Framework Core with SQLite
- ASP.NET Core Identity and role-based authorization
- Razor views and Bootstrap

## Run locally

Follow the [development setup guide](docs/DEVELOPMENT.md) to restore dependencies, create an isolated local database, apply migrations, and start the app.

## Project map

See [the project overview](docs/PROJECT_OVERVIEW.md) for the main areas, data flow, and user journeys.

## Data and deployment

This is an academic prototype, not a production-ready service. Use synthetic data during development. The repository history contains a tracked SQLite database and image assets whose data provenance has not been verified; do not reuse them as demo data. The app stores uploaded images in the web project, so deployments need an explicit plan for database and image storage, backups, access control, and operational monitoring. The repository does not provide default administrator credentials.
