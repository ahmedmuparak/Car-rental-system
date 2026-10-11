# Car Rental System 

A backend REST API for a car rental platform built with **ASP.NET Core Web API**. The system provides car and branch management, booking and payment workflows, authentication, reviews, favorites, notifications, and administrative features.

**GitHub Repository:** [Car-rental-system](https://github.com/ahmedmuparak/Car-rental-system)

##  Features

- User registration, login, and profile management.
- JWT-based authentication and role-based authorization.
- Password reset and OTP verification workflow.
- Car and rental branch management.
- Booking creation, cancellation, and status management.
- Stripe payment integration.
- Car search, filtering, and pagination.
- Favorites and car reviews/ratings.
- Conversations and messaging with SignalR support.
- Notifications for application events.
- Admin dashboard and statistics.
- Distributed caching for selected API data.

##  Technologies Used

- **C#**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **Linq**
- **ASP.NET Core Identity**
- **JWT Authentication**
- **Swagger / OpenAPI**
- **SignalR** for real-time communication
- **Stripe** for payment integration
- **Redis-compatible distributed caching**
- **AutoMapper and DTOs**, where configured
- **Git & GitHub**

## 🏗️ Architecture

The solution follows a layered architecture to separate responsibilities and improve maintainability.

```text
Car Rental System
│
├── CarRental.API
│   ├── Controllers
│   ├── Middleware
│   └── API Configuration
│
├── CarRental.Application
│   ├── Services
│   ├── DTOs
│   └── Interfaces
│
├── CarRental.Infrastructure
│   ├── Database Context
│   ├── Repositories
│   └── External Integrations
│
└── CarRental.Domain
    ├── Entities
    └── Enums
```

*Note: Adjust the folder names above if they differ from the actual solution structure.*

##  Main Modules

| Module | Description |
|---|---|
| Authentication & Profile | Registration, login, user information, and password reset |
| Cars | Car management and related operations |
| Branches | Rental branch management |
| Bookings | Create, cancel, and manage reservations |
| Payments | Payment processing integration with Stripe |
| Search & Filtering | Search for cars using supported filters and pagination |
| Favorites | Manage favorite cars |
| Reviews & Ratings | Submit and retrieve car reviews |
| Conversations & Messages | Manage conversations and messages |
| Notifications | Notify users about relevant application events |
| Admin Dashboard | Dashboard overview and system statistics |

##  Prerequisites

Before running the project, make sure you have:

- .NET SDK compatible with the project's target framework.
- SQL Server.
- Git.
- Visual Studio or Visual Studio Code.
- Stripe credentials if you want to test payment integration.
- A Redis-compatible cache service if distributed caching is enabled.

##  Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/ahmedmuparak/Car-rental-system.git
cd Car-rental-system
```

### 2. Restore dependencies

Open a terminal in the directory containing the solution file and run:

```bash
dotnet restore
```

### 3. Configure application settings

Configure the settings required by the application, including:

- SQL Server connection string.
- JWT authentication settings.
- SMTP/email settings, if enabled.
- Stripe API keys, if enabled.
- Distributed-cache connection settings, if enabled.

Use environment variables, .NET User Secrets, or a secrets manager for sensitive values. Never commit real passwords, API keys, or signing secrets to GitHub.

### 4. Apply database migrations

Locate the project containing the Entity Framework Core `DbContext` and the API startup project.

Run the following command from the solution directory, replacing the placeholders with the actual project paths:

```bash
dotnet ef database update --project <InfrastructureProjectPath> --startup-project <ApiProjectPath>
```

If the migrations are stored in another project, adjust the command accordingly. Ensure the EF Core CLI tool and required design package are installed.

### 5. Run the API

Replace `<ApiProjectPath>` with the path to `CarRental.API.csproj`:

```bash
dotnet run --project <ApiProjectPath>
```

The terminal will display the local URLs where the API is running.

##  Testing with Swagger

The API can be tested using Swagger without a separate frontend.

1. Run the API.
2. Open the Swagger URL displayed in the terminal.
3. Register a test account using the authentication endpoint.
4. Log in and copy the returned JWT token.
5. Click **Authorize** in Swagger, if Bearer authentication is configured.
6. Enter the token as requested by the Swagger interface.
7. Test the available endpoints according to the user's permissions and roles.

Use a development database and test data when trying operations that create, update, or delete records.

##  Authentication & Authorization

The API uses JWT-based authentication and role-based authorization where configured.

Protected endpoints may require a valid access token and a specific role. Never publish access tokens or sensitive authentication details.

##  Real-Time Messaging

The project includes conversation and message functionality with SignalR support. Refer to the configured hub class and API setup for the exact hub URL and connection requirements.

##  Caching

Distributed caching is used for selected API data to reduce repeated database queries and improve response times.

Cache expiration and invalidation should be configured appropriately to prevent stale data after updates or deletions.

##  Security

- Keep credentials and secrets out of source control.
- Use HTTPS in deployed environments.
- Validate incoming requests on the server.
- Apply authorization checks to protected operations.
- Verify Stripe webhook signatures when handling payment events.
- Rotate any secret that has accidentally been committed to a public repository.

