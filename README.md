# 🚗 Car Rental System API

A RESTful backend for a car rental platform, built with **ASP.NET Core 9** and **Clean Architecture**. It covers the full rental lifecycle: authentication with email OTP, car and branch management, bookings, Stripe payments, reviews, favorites, real-time chat, notifications, and an admin dashboard.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?logo=redis&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-635BFF?logo=stripe&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-real--time-blue)

---

## 📑 Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Roles & Permissions](#-roles--permissions)
- [API Overview](#-api-overview)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Real-Time Chat](#-real-time-chat-signalr)
- [Payments (Stripe)](#-payments-stripe)
- [Security Notes](#-security-notes)
- [Author](#-author)

---

## ✨ Features

- **Authentication & Identity**: registration with email verification (OTP), login, password reset via OTP, change password, profile management, JWT bearer tokens.
- **Role-based authorization**: six roles (`SuperAdmin`, `Admin`, `CarOwner`, `Employee`, `Customer`, `Driver`) seeded automatically on startup.
- **Cars**: full CRUD, availability and status management (`Available`, `Reserved`, `Rented`, `Maintenance`, `Unavailable`), search and filtering with pagination, automatic car image lookup.
- **Branches**: manage rental branches and browse cars by branch.
- **Bookings**: create, confirm, complete, and cancel reservations with a clear status flow (`Pending → Confirmed → Active → Completed`, plus `Cancelled` / `Rejected`).
- **Payments**: Stripe Checkout sessions and webhook handling; payment statuses `Pending`, `Paid`, `Failed`, `Refunded`.
- **Reviews & Ratings**: customers review cars; reviews can be listed per car or per user.
- **Favorites**: add, remove, list, and clear favorite cars.
- **Chat**: one-to-one conversations and messages (REST + SignalR hub for real-time delivery).
- **Notifications**: booking, payment, car, message, and system notifications with unread counters.
- **Admin Dashboard**: overview and statistics for users, cars, bookings, payments, and recent bookings.
- **Caching**: Redis-backed distributed cache for frequently read data.
- **Global exception handling**: consistent `ProblemDetails` error responses.
- **Swagger / OpenAPI** with JWT authorization built in.

---

## 🧰 Tech Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 9) |
| Language | C# |
| ORM / Database | Entity Framework Core 9, SQL Server |
| Auth | ASP.NET Core Identity, JWT Bearer |
| Real-time | SignalR |
| Caching | Redis (StackExchange.Redis, `IDistributedCache`) |
| Payments | Stripe.net |
| Email | MailKit (SMTP) |
| Docs | Swashbuckle (Swagger UI) |

---

## 🏗️ Architecture

The solution is split into four projects with dependencies pointing inward:

```text
Car Rental System.sln
│
├── CarRental.API              # Presentation layer
│   ├── Controllers            # Auth, Car, Branch, Booking, Payment, Review,
│   │                          # FavoriteList, Conversation, Message,
│   │                          # Notification, AdminDashboard
│   ├── Middleware             # GlobalExceptionMiddleware
│   └── Program.cs             # DI, auth, CORS, Swagger, SignalR, pipeline
│
├── CarRental.Application      # Contracts & business models
│   ├── DTOs                   # Request/response models per module
│   ├── Interfaces             # Service abstractions (IAuthService, IBookingService, ...)
│   ├── Exceptions             # BadRequest, Conflict, Forbidden, NotFound
│   └── Configuration          # Options classes
│
├── CarRental.Infrastructure   # Implementations
│   ├── Persistence            # CarRentalDbContext
│   ├── Migrations             # EF Core migrations
│   ├── Services               # Service implementations (Auth, Booking, Payment, Redis, ...)
│   ├── Identity               # ApplicationUser, AppRoles, IdentitySeeder
│   └── Hubs                   # ChatHub (SignalR)
│
└── CarRental.Domain           # Core entities & enums
    ├── Entities               # Car, Branch, Booking, Payment, Review,
    │                          # FavoriteList, Conversation, Message, Notification
    └── Enums                  # BookingStatus, CarStatus, PaymentStatus, ...
```

---

## 🔐 Roles & Permissions

| Role | Typical capabilities |
|---|---|
| **SuperAdmin** | Everything, including assigning roles to users |
| **Admin** | Manage branches, cars, users, bookings; access the dashboard |
| **CarOwner** | Create, update, delete cars and change car status |
| **Employee** | View and manage bookings (confirm, complete, update payment) |
| **Customer** | Create and cancel own bookings, pay, review cars |
| **Driver** | Reserved role |

On first run the app seeds all roles and a **SuperAdmin** account from configuration (see [Configuration](#-configuration)).

---

## 📡 API Overview

All routes are prefixed with `/api`. Full interactive documentation is available in Swagger.

<details>
<summary><b>Auth</b> — <code>/api/auth</code></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | `/register` | Register a new account | Public |
| POST | `/verify-email` | Verify email with OTP | Public |
| POST | `/login` | Login and receive a JWT | Public |
| POST | `/forgot-password` | Request a password-reset OTP | Public |
| POST | `/verify-otp` | Verify the reset OTP | Public |
| POST | `/reset-password` | Set a new password | Public |
| GET / PUT | `/user-info` | Get or update own profile | Authenticated |
| POST | `/change-password` | Change password | Authenticated |
| POST | `/logout` | Logout | Authenticated |
| POST | `/assign-role` | Assign a role to a user | SuperAdmin |
| GET | `/all-users`, `/user-by-email` | Browse users | Admin |

</details>

<details>
<summary><b>Cars</b> — <code>/api/car</code></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | `/` | List cars | Public |
| GET | `/{id}` | Car details | Public |
| GET | `/available` | Available cars | Public |
| GET | `/branch/{branchId}` | Cars by branch | Public |
| GET | `/search` | Search / filter with pagination | Public |
| POST | `/` | Create a car | SuperAdmin, Admin, CarOwner |
| PUT | `/{id}` | Update a car | SuperAdmin, Admin, CarOwner |
| PUT | `/{id}/status` | Change car status | SuperAdmin, Admin, CarOwner |
| DELETE | `/{id}` | Delete a car | SuperAdmin, Admin, CarOwner |

</details>

<details>
<summary><b>Branches</b> — <code>/api/branch</code></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | `/GetAllBranches` | List branches | Public |
| GET | `/GetBranchById/{id}` | Branch details | Public |
| POST | `/CreateBranch` | Create a branch | SuperAdmin, Admin |
| PUT | `/UpdateBranch/{id}` | Update a branch | Admin |
| DELETE | `/DeleteBranch/{id}` | Delete a branch | SuperAdmin, Admin |

</details>

<details>
<summary><b>Bookings</b> — <code>/api/booking</code></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | `/` | List all bookings | SuperAdmin, Admin, Employee |
| GET | `/{id}` | Booking details | Staff, Customer |
| GET | `/user` | Current user's bookings | Authenticated |
| POST | `/` | Create a booking | Customer |
| PATCH | `/cancel/{id}` | Cancel a booking | Staff, Customer |
| PATCH | `/confirm/{id}` | Confirm a booking | Staff |
| PATCH | `/complete/{id}` | Complete a booking | Staff |
| PATCH | `/payment/{id}` | Update payment info | Staff |

</details>

<details>
<summary><b>Payments</b> — <code>/api/payment</code></summary>

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | `/create-checkout-session` | Create a Stripe Checkout session | Customer |
| POST | `/webhook` | Stripe webhook receiver | Stripe |

</details>

<details>
<summary><b>Reviews, Favorites, Chat, Notifications, Dashboard</b></summary>

| Module | Endpoints |
|---|---|
| **Review** `/api/review` | `POST /` · `GET /car/{carId}` · `GET /user` · `GET /{reviewId}` · `DELETE /{reviewId}` |
| **FavoriteList** `/api/favoritelist` | `POST /` · `GET /` · `DELETE /{carId}` · `DELETE /clear` |
| **Conversation** `/api/conversation` | `POST /` · `GET /my-conversations` |
| **Message** `/api/message` | `GET /conversation/{id}` · `POST /conversation/{id}` · `GET /unread` · `PUT /{messageId}` · `DELETE /{messageId}` |
| **Notification** `/api/notification` | `GET /user` · `GET /unread` · `GET /unread-count` · `POST /mark-as-read/{id}` |
| **AdminDashboard** `/api/admindashboard` | `GET /overview` · `/users-statistics` · `/cars-statistics` · `/bookings-statistics` · `/payments-statistics` · `/recent-bookings` |

</details>

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB, Express, or full)
- A Redis instance (local, Docker, or a hosted service such as Upstash). **Required**: the app will not start without a Redis connection string.
- A Stripe account (test mode is fine) for payments
- An SMTP account (e.g. a Gmail app password) for OTP emails
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/ahmedmuparak/Car-rental-system.git
cd Car-rental-system
```

### 2. Configure secrets

Use **User Secrets** for local development (never commit real values):

```bash
cd CarRental.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=CarRentalDB;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
dotnet user-secrets set "JwtSettings:Key" "<a-long-random-secret-at-least-32-chars>"
dotnet user-secrets set "EmailSettings:Email" "you@example.com"
dotnet user-secrets set "EmailSettings:Password" "<smtp-app-password>"
dotnet user-secrets set "SuperAdmin:Email" "admin@example.com"
dotnet user-secrets set "SuperAdmin:Password" "<StrongPassword!123>"
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
dotnet user-secrets set "Stripe:SuccessUrl" "http://localhost:5500/success.html"
dotnet user-secrets set "Stripe:CancelUrl" "http://localhost:5500/cancel.html"
```

See the full list of keys in [Configuration](#-configuration).

### 3. Create the database

From the solution root:

```bash
dotnet ef database update --project CarRental.Infrastructure --startup-project CarRental.API
```

### 4. Run

```bash
dotnet run --project CarRental.API --launch-profile https
```

| | URL |
|---|---|
| API | `https://localhost:7065` · `http://localhost:5212` |
| Swagger UI | `https://localhost:7065/swagger` |
| SignalR hub | `/hubs/chat` |

### 5. Try it in Swagger

1. Log in with the seeded SuperAdmin (or register a new account and verify the email OTP).
2. Copy the `token` from the login response.
3. Click **Authorize** and paste the JWT.
4. Call any endpoint allowed for your role.

---

## ⚙️ Configuration

| Section | Keys | Purpose |
|---|---|---|
| `ConnectionStrings` | `DefaultConnection`, `Redis` | SQL Server and Redis |
| `JwtSettings` | `Key`, `Issuer`, `Audience`, `DurationInMinutes` | Token signing and validation |
| `EmailSettings` | `Email`, `DisplayName`, `Password`, `Host`, `Port` | SMTP for OTP emails |
| `SuperAdmin` | `Email`, `Password` | Account seeded at startup |
| `Stripe` | `SecretKey`, `PublishableKey`, `WebhookSecret`, `SuccessUrl`, `CancelUrl` | Payments |
| `CarImages` | `BaseUrl`, `ApiKey`, `ApiSecret` | External car image provider (signed image URLs) |

**CORS:** the `FrontendPolicy` currently allows `http://localhost:5500/5501` and `http://127.0.0.1:5500/5501`. Update the origins in `Program.cs` for your frontend.

---

## 💬 Real-Time Chat (SignalR)

- Hub endpoint: `/hubs/chat` (JWT required)
- Join a conversation group with the hub method `JoinConversation(conversationId)`; only participants of the conversation are allowed.
- Messages are sent through `POST /api/message/conversation/{id}`, saved to the database, and pushed live to connected participants via the `ReceiveMessage` event.

---

## 💳 Payments (Stripe)

1. A **Customer** creates a booking.
2. The client calls `POST /api/payment/create-checkout-session` and redirects the user to the returned Stripe Checkout URL.
3. After checkout, Stripe calls `POST /api/payment/webhook`; the payment record and the booking's payment status are marked as `Paid`.

For local webhook testing, use the Stripe CLI:

```bash
stripe listen --forward-to https://localhost:7065/api/payment/webhook
```

Copy the printed signing secret into `Stripe:WebhookSecret`.

---

## 🔒 Security Notes

- Keep all secrets (JWT key, SMTP password, Stripe keys, Redis password, SuperAdmin password) out of source control: use User Secrets locally and environment variables or a secret manager in production.
- Use a long, random JWT signing key and HTTPS in every deployed environment.
- Verify Stripe webhook signatures with `Stripe:WebhookSecret`.
- Rotate any credential that was ever committed to the repository.

---

## 👤 Author

GitHub: [@ahmedmuparak](https://github.com/ahmedmuparak)
