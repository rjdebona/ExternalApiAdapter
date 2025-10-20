# DebtCollectionPortal

DebtCollectionPortal is a demo ASP.NET Core (NET 8) application that serves a simple static frontend from `wwwroot/` and implements server-side controllers and application services that adapt and proxy requests to an external API (default base: https://api.example.com). The project uses a layered, domain-centered architecture (Onion / Domain-Centric) with HTTP adapters acting as an Anti-Corruption Layer.

---

## Functional Requirements

| ID  | Requirement | Description / Notes |
|-----|-------------|---------------------|
| RF1 | User registration | Calls external `/api/webuser/register` through `UserRepositoryApi`. Frontend: `registration.html`. |
| RF2 | Login / validation | Calls external `/api/webuser/validate` and returns the external token to the frontend. |
| RF3 | View account | Calls `GET /api/account/{accountId}` through `AccountRepositoryApi`. Frontend: `dashboard.html`. |
| RF4 | Update address | Calls `PUT /api/account/{accountId}/address` via `ClientRepositoryApi`. Frontend: `update-address.html`. |
| RF5 | Update email | Calls `PUT /api/account/{accountId}/email` via `ClientRepositoryApi`. Frontend: `update-email.html`. |

## Non-Functional Requirements

| ID  | Requirement | Description / Notes |
|-----|-------------|---------------------|
| RNF1 | API Security | API requests require `X-API-Key` header. `ApiKeyMiddleware` protects `/api/*` and accepts either `X-API-Key` or an `Authorization: Bearer <jwt>` header. |
| RNF2 | Configurability | External API base URL and API key are configurable via `appsettings.json` (`ExternalApi:BaseUrl`, `ExternalApi:ApiKey`) or environment variables. |
| RNF3 | Portability | Built with .NET 8 — runs on Windows, macOS, and Linux with .NET 8 SDK. |
| RNF4 | Observability | Infrastructure adapters log HTTP errors via `ILogger<>`. |

---

## Architectural Choices

- Layered / Domain-Centered architecture
  - Layers: Presentation (static pages + controllers), Application (services such as `UserService`, `AccountService`, `ClientService`), Domain (entities, value objects, repository interfaces), Infrastructure (HTTP adapters in `Infrastructure/ApiRepositories`).
  - The domain layer contains business rules and invariants; it does not depend on infrastructure.

- Onion / Domain-Centric style
  - Business logic and invariants live in domain entities (`Client`, `User`, `Account`) and value objects (`Email`, `AccountId`).
  - Application services orchestrate interactions between controllers and repository adapters.

- HTTP Adapters (Anti-Corruption Layer)
  - `AccountRepositoryApi`, `UserRepositoryApi`, and `ClientRepositoryApi` are `HttpClient`-backed adapters registered through DI. They translate external JSON payloads into domain objects and return `OperationResult` / `AuthResult` shapes used by the application.

- Configuration and DI
  - `Program.cs` registers typed `HttpClient` instances for each adapter and wires application services and repository interfaces.

- Frontend
  - Minimal static HTML pages live in `wwwroot/`. Inline scripts use `fetch` and `localStorage` (`authToken`, `accountId`). There is no bundler or SPA framework — keeping the UI minimal for the coding challenge.

---

## External API Contract (used by adapters)

- Base URL: `https://api.example.com` (default)
- POST `/api/webuser/register` — payload: `{ accountId, dateOfBirth, postcode, email, password }`
- POST `/api/webuser/validate` — payload: `{ email, password }` — returns `{ token }`
- GET `/api/account/{accountId}` — account payload (outstanding balance, debtor, dates, etc.)
- PUT `/api/account/{accountId}/address` — update address
- PUT `/api/account/{accountId}/email` — update email

Note: adapters normalize the responses; the application is account-centric.

---

## How to run (step-by-step)

Prerequisites

- .NET 8 SDK installed
- A terminal (PowerShell on Windows is used in examples)

1) **Configure the application**

Before running, you need to configure the external API settings. You can do this by:

**Option A: Using appsettings.Development.json (recommended for development)**
```json
{
  "ExternalApi": {
    "BaseUrl": "https://your-api-endpoint.com",
    "ApiKey": "your-actual-api-key"
  },
  "Jwt": {
    "Key": "your-secret-jwt-key-must-be-at-least-32-characters-long"
  }
}
```

**Option B: Using environment variables**
```powershell
$env:ExternalApi__BaseUrl = 'https://your-api-endpoint.com'
$env:ExternalApi__ApiKey = 'your-actual-api-key'
$env:Jwt__Key = 'your-secret-jwt-key-must-be-at-least-32-characters-long'
```

2) Build the project

```powershell
cd C:\Projetos\ExternalApiAdapter\DebtCollectionPortal
dotnet build
```

3) Run the application (development)

```powershell
# Starts Kestrel; default URLs are http://localhost:5000 and https://localhost:5001
dotnet run
```

You should see console output showing the app started and which URLs are served. Static frontend files are served from `wwwroot/`.

## Configuration

The application requires the following configuration:

- **ExternalApi:BaseUrl** - The base URL of your external API
- **ExternalApi:ApiKey** - Your API key for authentication
- **Jwt:Key** - Secret key for JWT token generation (minimum 32 characters)

⚠️ **Security Note**: Never commit real API keys or secrets to version control. Use `appsettings.Development.json` (which is in .gitignore) or environment variables for sensitive configuration.

---

## Manual test flows (browser)

The frontend stores `authToken` and `accountId` in `localStorage`.

1) Register
- Open `http://localhost:5000/registration.html`, fill the form and submit. The page calls `/api/webuser/register` through the adapter.

2) Login
- Open `http://localhost:5000/index.html` and login. The page calls `/api/webuser/validate`. On success the returned token is stored in `localStorage.authToken` and the user is redirected to `dashboard.html`.

3) Dashboard (view account)
- `dashboard.html` performs `GET /api/account/{accountId}` and renders account data.

4) Update address
- `update-address.html` submits to `PUT /api/account/{accountId}/address` (via `ClientRepositoryApi`).

5) Update email
- `update-email.html` submits to `PUT /api/account/{accountId}/email`.

---

Notes: 401 / 404 / 405 responses are expected in some cases (invalid credentials, missing account data, wrong endpoint). Check the `dotnet run` console logs for adapter-level errors.

---
