# Consultant Platform Backend

[![Backend CI/CD](https://github.com/mkatolika/consultant-platform-backend/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/mkatolika/consultant-platform-backend/actions/workflows/ci.yml)

A production-minded ASP.NET Core 8 REST API for a consultation marketplace. Clients can register, discover approved consultants and services, view availability, and create bookings. Consultants apply for approval, publish non-overlapping availability, and manage appointments. Administrators manage departments, services, roles, and consultant approvals.

This project demonstrates backend development as well as delivery engineering: API design, validation, JWT authentication, role-based authorization, Entity Framework Core modelling, automated tests, Docker packaging, security scanning, and deployment to Azure Container Apps.

## Business workflow

```text
User registers
      |
      +--> browses departments, services, approved consultants and open slots
      |
      +--> books a matching consultant/service/slot

User applies to become a consultant
      |
      +--> administrator reviews and approves the application
      |
      +--> role changes from User to Consultant
      |
      +--> consultant publishes availability and manages booking status
```

The API keeps feature responsibilities separate:

- **Authentication** creates Identity users and issues signed JWT access tokens.
- **Administration** protects role, department, service, and approval operations with the `Admin` role.
- **Consultants** can apply once, select existing services, publish their own availability, and access only their assigned bookings.
- **Clients** can only create bookings as the authenticated JWT user; a caller cannot impersonate another client by submitting a user ID.
- **Bookings** verify that the slot belongs to the requested approved consultant and that the consultant offers the requested service.

## Technical design

- ASP.NET Core 8 Web API and controller-based routing
- ASP.NET Core Identity for users, password policies, roles, failed-login lockout, and password hashing
- JWT bearer authentication with issuer, audience, lifetime, signature, and key-length validation
- Entity Framework Core 8 with SQL Server and code-first migrations
- DTO-based request contracts with Data Annotations and automatic `400 Bad Request` responses
- Role-based authorization and resource ownership checks
- Swagger/OpenAPI with JWT bearer authentication support
- xUnit and EF Core InMemory tests
- Docker multi-stage container build
- GitHub Actions CI/CD with CodeQL, Gitleaks, Trivy, OWASP ZAP, immutable image promotion, Azure OIDC, and Azure Container Apps deployment

## Data model

```text
AppUser 1 ----- 0..1 Consultant
   |                    |
   |                    +---- * ConsultantService * ---- 1 Service
   |                                                       |
   +---- * Slot                                          1 Department
   |
   +---- * Booking * ---- Consultant
                |
                +---- 1 Service
                +---- 1 Slot (unique, preventing double booking)
```

Important database constraints include unique consultant user IDs, licence numbers, department names, service names, and booking slot IDs. Prices use `decimal(18,2)` rather than floating-point values.

## API overview

| Area | Endpoint examples | Access |
|---|---|---|
| Authentication | `POST /api/Auth/register`, `POST /api/Auth/login` | Public |
| Services | `GET /api/Services` | Public |
| Consultants | `GET /api/Consultants/by-service/{serviceId}` | Public |
| Availability | `GET /api/Slots/by-consultant/{consultantId}` | Public |
| Bookings | `POST /api/Booking/create`, `GET /api/Booking/my-bookings` | Authenticated |
| Consultant application | `POST /api/Consultants/apply` | User |
| Consultant operations | `POST /api/Slots/create`, booking status operations | Consultant |
| Administration | department/service/role creation and consultant approval | Admin |

Swagger provides the complete live contract at `/swagger`. The application also exposes `/health` for container orchestration and deployment verification.

## Validation and security decisions

- Required request fields use `[Required]`, with length and numeric range constraints where appropriate.
- Slot end time must follow start time, and new slots must be in the future.
- A consultant cannot create overlapping availability.
- A client ID is derived from the authenticated token instead of trusted from request JSON.
- Login failures intentionally return the same response for an unknown user and a wrong password, reducing account enumeration.
- JWT secrets are not committed. Startup fails early when the database connection, signing key, issuer, audience, or CORS origins are missing.
- Admin and consultant endpoints use role authorization, while record-level ownership prevents one consultant from modifying another consultant's booking.
- The signing key must be Base64 and decode to at least 32 bytes.

## Local development

Requirements:

- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Docker (optional)

Restore the application, tests, and repository-local EF CLI:

```powershell
dotnet restore ConsultationApplication.sln
dotnet tool restore
```

Configure secrets without editing tracked settings:

```powershell
cd ConsultationApplication
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=ConsultationPlatform;Trusted_Connection=True;TrustServerCertificate=True"

$jwtBytes = [Text.Encoding]::UTF8.GetBytes("replace-this-with-a-random-32-byte-key")
$jwtKey = [Convert]::ToBase64String($jwtBytes)
dotnet user-secrets set "Jwt:Key" $jwtKey
```

Apply migrations and start the API from the repository root:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=(localdb)\MSSQLLocalDB;Database=ConsultationPlatform;Trusted_Connection=True;TrustServerCertificate=True"
dotnet tool run dotnet-ef database update --project ConsultationApplication
dotnet run --project ConsultationApplication
```

The roles `Admin`, `Consultant`, and `User` must exist before registration and approval workflows are used. In a deployed environment these should be provisioned by a controlled database initialization process rather than an anonymous API endpoint.

## Tests

```powershell
dotnet test ConsultationApplication.sln --configuration Release
```

The tests verify request validation and booking security behavior, including authenticated client ownership, slot/consultant consistency, required registration data, and invalid availability windows.

## Docker

```powershell
docker build -t consultant-platform-backend ConsultationApplication

docker run --rm -p 8080:8080 `
  -e ConnectionStrings__DefaultConnection="<connection-string>" `
  -e Jwt__Key="<base64-signing-key>" `
  -e Jwt__Issuer="ConsultantPlatform" `
  -e Jwt__Audience="ConsultantPlatformClient" `
  -e Cors__AllowedOrigins__0="http://localhost:3000" `
  consultant-platform-backend
```

## CI/CD and cloud delivery

The GitHub Actions workflow restores and builds the solution, creates one immutable Docker candidate, and reuses that same image for container scanning, dynamic security testing, publishing, and deployment. This avoids testing one image and deploying another.

```text
BUILD                     QUALITY & SECURITY                 RELEASE
restore/build ----------+ unit tests ---------------------+ version
Docker candidate -------+ formatting                     + publish exact image
                         + CodeQL / Gitleaks / Trivy       + deploy with Azure OIDC
                         + OWASP ZAP                       + verify image and /health
```

Deployment uses GitHub's Azure OIDC authentication rather than a long-lived Azure password. GitHub Environments provide manual approval gates for deployment and database migration.

Required GitHub secrets:

- `DOCKERHUB_USER_NAME`
- `DOCKERHUB_TOKEN` or `DOCKERHUB_PASSWORD`
- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `DB_CONNECTION_STRING` on the protected database-migration environment

Required GitHub variables:

- `AZURE_RESOURCE_GROUP`
- `AZURE_CONTAINER_APP_NAME`
- Optional `DOCKER_IMAGE_NAME` (defaults to `lwazi`)

## Engineering trade-offs and roadmap

The project deliberately keeps a single Web API project so the portfolio remains easy to run and review. As the product grows, the next improvements would be:

- separate application/domain/infrastructure layers;
- add refresh tokens and email verification;
- add pagination and cancellation-token support;
- use integration tests with a disposable SQL Server container;
- introduce optimistic concurrency for high-volume slot booking;
- seed roles and the first administrator through a dedicated initialization process;
- add structured logging, OpenTelemetry, and database-aware health checks;
- make all CI quality and security checks blocking after resolving the existing vulnerability backlog.

## What this project demonstrates

For a junior .NET role, this project shows that I can build more than CRUD endpoints. I can model real relationships, secure APIs with Identity and JWT, enforce business rules at both API and database levels, write automated tests, package services with Docker, and design a CI/CD pipeline that scans and promotes an immutable artifact to Azure.