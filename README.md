# 🚍 Ra7ala (رحالة) - Backend API

Ra7ala is a high-performance, enterprise-grade Transportation Management System (TMS) built on **.NET 9**. The backend follows a strict **Clean Architecture** pattern with Domain-Driven Design, ensuring long-term maintainability, security, and scalability.

---

## 🚀 Key Features & Capabilities

### 🔐 Security & Identity Management
- **ASP.NET Core Identity:** Custom implementation managing five user roles — `Owner`, `SuperAdmin`, `Admin`, `Driver`, `Passenger`.
- **Stateless JWT Authentication:** Secure, token-based auth with fine-grained role-based access control (RBAC).
- **Secure Data Handling:** Password hashing, multi-layered validation, and protection against common web vulnerabilities.
- **CORS Security:** Fine-tuned Cross-Origin Resource Sharing policies for secure frontend-backend communication.

### 📅 Intelligent Trip Engine
- **Multi-Station Routing:** Complex routes with multiple intermediate stops, departure times, and arrival windows.
- **High-Performance Search:** Filter trips by origin city, destination city, exact date, and required seat count.
- **Concurrency Protection:** Optimistic concurrency via `[Timestamp] RowVersion` on the `Trip` entity prevents overbooking under simultaneous requests.
- **Approved Partners Only:** Only trips from verified "Approved" companies are visible to passengers.

### 🎫 Booking & Ticketing Ecosystem
- **Atomic Operations:** Unit of Work and Repository patterns ensure all booking operations are transactional.
- **Unique Ticket Codes:** 6-character alphanumeric codes with collision detection and timestamp-based fallbacks.
- **Sequential Seat Assignment:** Seats are assigned incrementally per trip for group bookings.
- **Soft-Delete Architecture:** `IsDeleted` flag across all core entities for data safety and audit trails.

### 💳 Financial Infrastructure (Stripe)
- **PCI-Compliant Payments:** Secure Stripe API integration for global payment processing.
- **Server-Side Amount Enforcement:** Prevents client-side price tampering.
- **Automated Webhooks:** Syncs payment states (Success, Failure, Refund, Cancel) in real-time via `payment_intent.succeeded`.
- **Full Refund Pipeline:** Automated refund processing and booking reversal logic.

### 🤖 AI-Powered Customer Experience (Gemini AI)
- **Model:** Google Gemini 2.5 Flash-Lite (configurable via `appsettings.json`).
- **Live Trip Context:** The AI assistant receives a compact summary of all upcoming trips on every request.
- **Multilingual:** Detects and responds in the user's language and dialect automatically.
- **Intelligent Backoff:** Exponential backoff for API retries handles rate limits gracefully.

### 🚌 Fleet & Infrastructure Management
- **Amenity Tracking:** Detailed bus facilities (WiFi, AC, USB Charging, etc.).
- **Station Network:** Hierarchical city → station management with system-level and company-level stations.
- **Partner Dashboard:** Full management suite for transportation companies.

---

## 🏗️ Architecture

```
Ra7ala-api/
├── Domain/           # Entities, Enums, Repository Interfaces
├── Application/      # Services, DTOs, Business Logic
├── Infrastructure/   # EF Core, Repositories, External Services
└── Presentation/     # Controllers, Extensions, Program.cs
```

- **Clean Architecture** with strict layer separation
- **Repository Pattern** — Generic + specialized repositories
- **Unit of Work** — Transaction boundary management
- **DTO-Driven** — Clear separation between internal models and API contracts
- **Modular Extensions** — Each service registered via dedicated extension methods

---

## 🛠️ Technical Stack

| Layer | Technology |
|---|---|
| Framework | .NET 9 Web API |
| ORM | Entity Framework Core 9 (SQL Server) |
| Auth | ASP.NET Core Identity + JWT Bearer |
| AI | Google Gemini 2.5 Flash-Lite |
| Payments | Stripe .NET SDK v51 |
| Email | MailKit / SMTP |
| Docs | Swagger / OpenAPI |

---

## 🔑 Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": { "DefaultConnection": "<SQL Server connection string>" },
  "JWT": { "Key": "<secret>", "Issuer": "Ra7alaApi", "Audience": "Ra7alaClient", "DurationInDays": 7 },
  "Gemini": {
    "ApiKey": "<Google AI Studio API key>",
    "Model": "gemini-2.5-flash-lite",
    "Endpoint": "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent"
  },
  "Stripe": { "PublishableKey": "", "SecretKey": "", "WebhookSecret": "" },
  "EmailSettings": { "Email": "", "Password": "", "Host": "smtp.gmail.com", "Port": 587 }
}
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB or Express)
- Visual Studio 2022 (v17.12+) or VS Code

### Setup
1. **Clone the repository and configure** `appsettings.json` with your connection string and API keys.
2. **Initialize the database:**
   ```bash
   dotnet ef database update --project Infrastructure --startup-project Presentation
   ```
3. **Run the API:**
   ```bash
   dotnet run --project Presentation
   ```
4. **Swagger UI** is available at `https://localhost:{port}/swagger`.

---

## 📋 API Overview

| Controller | Base Route | Auth |
|---|---|---|
| Auth | `/api/auth` | Mixed |
| Trips | `/api/trips` | Mixed |
| Booking | `/api/booking` | Passenger |
| Payments | `/api/payments` | Mixed |
| Company | `/api/company` | Mixed |
| Bus | `/api/bus` | Admin/SuperAdmin |
| Routes | `/api/routes` | Admin/SuperAdmin |
| Station | `/api/station` | Mixed |
| City | `/api/city` | Mixed |
| ChatBot | `/api/chatbot` | Public |

---

**Crafted with precision by the Ra7ala Engineering Team**
