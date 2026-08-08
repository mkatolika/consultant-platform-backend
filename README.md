# Consultant Platform Backend

[![Backend CI/CD](https://github.com/mkatolika/consultant-platform-backend/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/mkatolika/consultant-platform-backend/actions/workflows/ci.yml)

ASP.NET Core 8 API for the Consultant and Services Platform. It provides JWT authentication, role-based authorization, consultant and service management, appointment booking, availability, and client booking operations.

## Technology

- ASP.NET Core 8 Web API
- Entity Framework Core and SQL Server
- ASP.NET Core Identity and JWT authentication
- xUnit and EF Core InMemory testing
- Docker
- GitHub Actions, CodeQL, Gitleaks, Trivy, and OWASP ZAP
- Azure Container Apps deployment

## Local development

Configure a local connection string and JWT settings with user secrets, environment variables, or an ignored `appsettings.Development.json`. Never use the development JWT key committed in `appsettings.json` for a deployed environment.

```bash
dotnet restore ConsultationApplication.sln
dotnet restore ConsultationApplication.Tests/ConsultationApplication.Tests.csproj
dotnet run --project ConsultationApplication/ConsultationApplication.csproj
```

Health endpoint: `GET /health`

## Tests

```bash
dotnet test ConsultationApplication.Tests/ConsultationApplication.Tests.csproj --configuration Release
```

## Docker

```bash
docker build -t consultant-platform-backend ConsultationApplication
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="<connection-string>" \
  -e Jwt__Key="<base64-signing-key>" \
  -e Jwt__Issuer="<issuer>" \
  -e Jwt__Audience="<audience>" \
  consultant-platform-backend
```

## CI/CD

```text
BUILD                     SANITY CHECKS                  ARTIFACT PUBLISH          DEPLOY
dotnet-restore-build ---+ backend-unit-test ----------+ backend-version-bump --+
                         + backend-lint                 |                         + deploy-dev
backend-docker-candidate + backend-sast                 | backend-docker-publish -+
                         + backend-secret-detection     |
                         + backend-dependency-scanning  |
                         + backend-container-scanning   |
                         + backend-dast ----------------+
```

The Docker candidate is built exactly once, saved as a workflow artifact, loaded for container scanning and DAST, then retagged and pushed without rebuilding. Version bump and Docker publishing run in parallel. Deployment waits for both and verifies the immutable image plus `/health` endpoint.

All sanity jobs currently use `continue-on-error: true`, so they report failures without blocking image promotion or deployment. This should be tightened as the test and vulnerability backlog is resolved.

Default Docker Hub image:

```text
docker.io/<DOCKERHUB_USER_NAME>/lwazi:1.0.<run-number>
```

Required GitHub secrets:

- `DOCKERHUB_USER_NAME`
- `DOCKERHUB_TOKEN` or `DOCKERHUB_PASSWORD`
- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

Required GitHub variables:

- `AZURE_RESOURCE_GROUP`
- `AZURE_CONTAINER_APP_NAME`

Optional variable: `DOCKER_IMAGE_NAME`, defaulting to `lwazi`.