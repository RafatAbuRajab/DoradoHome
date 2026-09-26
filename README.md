# DoradoHome

A full-stack real estate platform built with **ASP.NET Core MVC** that connects **Clients** looking to buy or rent properties with **Agents (Employees)** who manage listings and deals, plus an **Admin** portal for managing the whole system.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Tech Stack](#️-tech-stack)
- [Project Structure](#-project-structure)
- [Key Features](#-key-features)
- [Data Model](#-data-model)
- [Requirements](#️-requirements)
- [Getting Started Locally](#-getting-started-locally)
- [Securing Credentials](#-securing-credentials)
- [Roles & Permissions](#-roles--permissions)
- [Security Notes](#️-security-notes)

---

## 🏠 Overview

DoradoHome provides three separate portals (ASP.NET Core Areas), one per user type:

| Portal | Target Users | Description |
|---|---|---|
| **Clients** | End users | Browse properties, save favorites, book viewing appointments, track offers and rentals |
| **Agent** | Real estate agent | Manage listed properties (sale/rent), handle appointments, submit offers, view sales history |
| **Admin** | System administrator | Manage agents, properties, appointments, rentals, roles/permissions, system settings |

---

## 🛠️ Tech Stack

| Component | Technology |
|---|---|
| Framework | ASP.NET Core MVC — **.NET 8.0** |
| Database | SQL Server (via Entity Framework Core 8) |
| ORM | Entity Framework Core 8.0.29 (Code-First + Migrations) |
| Auth & Authorization | ASP.NET Core Identity (with custom `AppUser` and `AppRole`) |
| Frontend | Razor Views (CSHTML), Bootstrap 5, jQuery, jQuery Validation |
| Session/Caching | Session + In-Memory Cache |

---

## 📂 Project Structure

The project uses **ASP.NET Core Areas** to separate the logic for each user type:

```
DoradoHome/
├── Areas/
│   ├── Admin/          # Admin dashboard (Agents, Properties, Rentals, Roles, Settings)
│   ├── Agent/           # Agent dashboard (Listings, Appointments, Sales, Rentals)
│   ├── Clients/         # Client dashboard (Properties, Appointments, Saved, Rentals)
│   └── Identity/        # Authentication (Login, Register, Forgot/Reset Password)
├── Controllers/
│   └── HomeController.cs        # Public pages (Home, Buy, Rent, About, Contact)
├── Data/
│   ├── AppDbContext.cs           # Main DbContext (inherits IdentityDbContext)
│   ├── AppUser.cs / AppRole.cs   # Identity extended with custom fields
│   └── DbSeeder.cs               # Seeds initial data (Amenities) + auto-runs migrations
├── Migrations/           # EF Core migration history
├── Models/               # Core entities (Property, Client, Employee, Appointment...)
├── Views/                # Public site pages
├── Program.cs            # Entry point and service configuration (Startup)
└── appsettings.json      # Connection string and email settings
```

---

## ✨ Key Features

### 🔑 Authentication (Identity Area)
- Account registration and login
- Password recovery via verification code (Forgot Password → Verify Code → Reset Password)
- Strong password policy (8 characters, uppercase + lowercase + digit + special character)
- Account lockout after 5 failed attempts (30-minute lockout)

### 🏘️ Property Management
- Multiple property types: Apartment, Villa, House, Land, Office, Shop, Warehouse, Commercial/Residential Building, Farm, Chalet, Hotel, Factory, Garage
- Property statuses: Active, Pending, Sold, Rejected
- Multiple images per property (`PropertyImage`) with a designated main image
- Amenities linked via a many-to-many relationship (`PropertyAmenity`)
- Auto-calculated price per square foot (`PricePerSquare`)

### 📅 Appointments System
- Viewing types: In-Person Tour, Virtual Tour, Phone Call
- Appointment statuses: Confirmed, Pending, Rejected, Closed, Pending Confirmation
- Rescheduling support from both the Agent and Admin sides

### 💰 Offers & Deals
- Sale offers and rental offers (`OfferType`)
- Offer status tracking: Pending, Accepted, Declined
- Offers linked to the appointment, property, client, and responsible employee

### 🏢 Rentals & Sales
- Full rental contracts: lease start/end date, monthly rent, security deposit
- Lease statuses: Active, Pending Signature, Ending Soon, Ended, Expired
- Sales history (`Sale`) linked to property, client, and employee

### ⭐ Saved Properties
- Clients can save properties as favorites to revisit later

### 👥 User Management (Admin)
- Manage agents (add, edit, view, activate/deactivate)
- Manage roles and permissions (Create Role, Assign Role, Edit Role)
- Account security settings and password change

---

## 🗃️ Data Model

Key entities and their relationships:

- **Property** ← (1:N) → `PropertyImage`, `Appointment`, `Rental`, `Sale`, `Offer`
- **Property** ← (N:N) → `Amenity` (via `PropertyAmenity`)
- **Property** ← (N:N) → `Client` (via `SavedProperty`)
- **Client / Employee** ← linked to `AppUser` (Identity) via `UserId`
- **Appointment** ← (1:N) → `Offer`
- **AppUser** extends Identity's defaults with `FirstName`, `LastName`, `IsActive`
- **AppRole** extends Identity's defaults with `Description`

---

## ⚙️ Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- SQL Server (LocalDB / Express / Full) — or any instance reachable via a compatible connection string
- Visual Studio 2022 (17.8+) or VS Code with the C# Dev Kit

---

## 🚀 Getting Started Locally

### 1. Clone the repository

```bash
git clone <repository-url>
cd DoradoHome
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Configure the database connection string

Update `appsettings.json` (or, better, use User Secrets as shown [below](#-securing-credentials)):

```json
"ConnectionStrings": {
  "DbConnection": "Server=.;Database=DoradoHome;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 4. Apply migrations and create the database

> Note: the project calls `DbSeeder.SeedAsync()` automatically on startup, which in turn runs `Database.MigrateAsync()` — meaning the database and tables are created automatically on first run. You can also apply migrations manually:

```bash
dotnet ef database update
```

### 5. Run the project

```bash
dotnet run
```

Or run directly from Visual Studio with **F5** / **IIS Express**.

The browser will open on the home page. You can reach each portal at:

- `/Identity/User/Login` — Sign in
- `/Clients/Dashboard/ClientDashboard` — Client portal
- `/Agent/Dashboard/AgentDashboard` — Agent portal
- `/Admin/Dashboard/Dashboard` — Admin portal

---

## 🔐 Securing Credentials

The project needs two sensitive settings: the **database connection string** and **email account credentials** (used to send password-reset codes). The repository's `appsettings.json` already ships with placeholder values (`"Your Email"`, `"Your App Password"`) instead of real credentials — replace them locally with your own values, and avoid committing real secrets back into this file.

### Recommended for development: .NET User Secrets

```bash
cd DoradoHome
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DbConnection" "Server=.;Database=DoradoHome;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "EmailSettings:Email" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"
dotnet user-secrets set "EmailSettings:Host" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:Port" "587"
```

These values are read automatically by `IConfiguration` the same way `appsettings.json` is, with no extra code changes needed.

### For Production

Use **Environment Variables**, **Azure Key Vault**, or a similar secrets-management service. Never rely on `appsettings.json` to store real credentials in production.

### Specifically for Gmail

The value required for `EmailSettings:Password` is an **App Password** (not your regular account password) — generated from your Google Account security settings (requires 2-Step Verification to be enabled first).

---

## 👤 Roles & Permissions

The system relies on `ASP.NET Core Identity` with custom entities:

- **AppUser**: adds `FirstName`, `LastName`, `FullName`, `IsActive` on top of `IdentityUser`
- **AppRole**: adds `Description` on top of `IdentityRole`

Roles are managed dynamically from `Admin/Roles` (create, edit, assign roles to users), and both `Client` and `Employee` are linked to an `AppUser` account via `UserId`.

Access to each Area is enforced with `[Authorize(Roles = "...")]` at the controller level:

- Every controller under `Areas/Admin` requires the `Admin` role.
- Every controller under `Areas/Agent` requires the `Agent` role.
- Every controller under `Areas/Clients` requires the `Client` role.
- `Areas/Identity` and the public `HomeController` stay open (login/registration and public pages), with `HomeController` applying `[Authorize(Roles = "Client")]` per-action on the endpoints that need a signed-in client (e.g. scheduling a viewing).

---

## ⚠️ Known Limitations / Future Improvements

- Email confirmation is currently disabled.
- Production secrets should be moved to a dedicated secret manager.
- Additional authorization policies can be introduced as the system grows.
  
---

## 📄 License

To be determined based on project needs.
