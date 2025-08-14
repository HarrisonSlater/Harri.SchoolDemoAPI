# Azure DevOps SchoolDemoAPI Build Pipeline
[The pipeline is defined in yaml here](./azure-pipelines.yml)
with other pipeline resources in folders

## Table of Contents

* Overview
* Stage 1 — Build
* Stage 2 — Build SQL Database (main only)
* Stage 3 — Deploy & Test (.NET in-agent)
* Stage 4 — Deploy & Test (Docker in-agent)
* Stage 5 — Publish Docker (main only)
* Stage 6 — Publish NuGet (main only)
* Stage 7 — Post GitHub Commit Status
* Troubleshooting notes

---

## Overview

* Build the .NET 8 REST API and its supporting libraries, 
* Produce Docker + NuGet artifacts, 
* Run Unit/Contract/Integration/E2E tests in the agent,
* and (on `main` branch) 
   * Publish Docker images and NuGet packages, 
   * Build and push SQL database as a pre-seeded Docker image.

### Key Features
- Comprehensive testing (Unit, Integration, Contract, E2E)
- Docker containerization
- NuGet package publishing
- GitHub commit status integration
- Database migration support

> In this repo, “Deploy & Test” stages runs 'in-agent' to keep costs at zero with the microsoft hosted agents monthly free minutes

> For a more realistic test you should deploy first to a production like environment and then begin a test stage

---

## Stage 1 — Build



This stage restores, builds, unit tests, contract tests, and publishes artifacts.

> Code coverage collection uses the [`CodeCoverage.runsettings`](/CodeCoverage.runsettings) file at the repo root. 

**Artifacts Created**:
- `build`: .NET binary zip for standard deployment
- `dockerBuild`: .NET Docker image for container deployment
- `databaseMigrations`: Database migration executable
- `pact`: Contract tested pact file
- `nuget`: NuGet packages to publish

---

## Stage 2 — Build SQL Database *(main only)*
Builds a SQL Server Docker image ([DockerFile](./docker/SQL-Database-Dockerfile)), seeds with tables and test data, and pushes to Docker Hub.

**Artifacts Used**:
- `databaseMigrations`

---

## Stage 3 — Deploy & Test (.NET in-agent)

Starts the API **as a .NET process** on the agent and runs Integration + E2E tests against it and a local SQL container.

This stage runs  `databaseMigrations` package before integration tests so pull request database changes can be verified.

**Artifacts Used**:
- `build`
- `databaseMigrations`

> Code coverage is collected for Integration tests to cover the respository classes
---

## Stage 4 — Deploy & Test (Docker in-agent)

Starts the API **as a Docker container** on the agent and runs Integration + E2E tests against it and a local SQL container.

This stage runs  `databaseMigrations` package before integration tests so pull request database changes can be verified.

**Artifacts Used**:
- `dockerBuild`
- `databaseMigrations`
---

## Stage 5 — Publish Docker *(main only)*

Pushes both the build number tag and `latest` for the API image to Docker Hub.

**Artifacts Used**:
- `dockerBuild`

---

## Stage 6 — Publish NuGet *(main only)*

Pushes symbols packages for the three libraries to NuGet.org, skipping duplicates.

> Nuget versions are currently updated manually

---

## Stage 7 — Post GitHub Commit Status (always)

Posts **success** or **failure** to the source commit on GitHub by calling a shared template:

* Template: `pipeline/tasks/rest-github-commit-check.yml`
* **Success job** runs when both “Deploy & Test” stages succeeded.
* **Failure job** runs when either “Deploy & Test” stage failed/skipped.

---

## Troubleshooting notes

* **Health checks**

  * REST API health is validated via `/health` before tests proceed.
  * Containers are waited on using `pipeline/scripts/wait-for-healthy-container.sh`.

---

## Why two “Deploy & Test” stages?

* **Stage 3** validates the API as it would run via `dotnet` publish output (no container).
* **Stage 4** validates the **Dockerized** container, ensuring parity between raw .NET and containerized runtime.

   This serves as a demonstration on how to build and test (in-agent) with or without containers.
---