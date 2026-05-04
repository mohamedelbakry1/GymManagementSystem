# 🏋️ Gym Management System

A full-featured **Gym Management System** built with **ASP.NET Core MVC (.NET 10)** following **N-Tier Architecture**. The system provides a complete solution for managing gym members, trainers, sessions, memberships, bookings, and subscription plans — with role-based authentication and authorization powered by **ASP.NET Core Identity**.

---

## ✨ Features

- **Member Management** — Add, update, delete, and view gym members with detailed profiles including health records and addresses.
- **Trainer Management** — Manage trainers with specialties and assigned sessions.
- **Session Management** — Create and schedule training sessions linked to trainers and categories.
- **Membership Management** — Track member subscriptions, plan assignments, and membership status.
- **Booking System** — Allow members to book sessions with full CRUD operations.
- **Plan Management** — Define and manage subscription plans with pricing and duration.
- **User & Account Management** — Register, login, and manage user accounts with role-based access control.
- **Analytics Dashboard** — View gym performance insights and statistics.
- **File Upload Support** — Attach files and images (e.g., member photos) via a dedicated attachment service.
- **Data Seeding** — Auto-seed initial data and identity roles/users on application startup.

---

## 🏗️ Architecture

The project follows a clean **3-Tier (N-Tier) Architecture**, separating concerns across three layers:

```
GymManagementSystem/
├── GymManagementDAL/    → Data Access Layer
├── GymManagementBLL/    → Business Logic Layer
└── GymManagementPL/     → Presentation Layer (MVC)
```

### 📦 GymManagementDAL (Data Access Layer)

Handles all database interactions using **Entity Framework Core** with **SQL Server**.

| Folder         | Description                                              |
|----------------|----------------------------------------------------------|
| `Entities/`    | Domain models: Member, Trainer, Session, Plan, etc.      |
| `Entities/Enums/` | Enumerations like Gender and Specialties              |
| `Data/`        | DbContext, configurations, and data seeding               |
| `Repositories/`| Repository pattern implementation (Interfaces & Classes) |

### ⚙️ GymManagementBLL (Business Logic Layer)

Contains all business rules, service interfaces/implementations, and mapping profiles.

| Folder           | Description                                          |
|------------------|------------------------------------------------------|
| `Services/Interfaces/` | Service contracts (e.g., IMemberService, IBookingService) |
| `Services/Classes/`    | Concrete service implementations                   |
| `Services/AttachmentService/` | File upload & management service            |
| `ViewModels/`          | Data transfer objects for each module              |
| `MappingProfile.cs`    | AutoMapper profile for entity ↔ ViewModel mapping  |

### 🖥️ GymManagementPL (Presentation Layer)

The ASP.NET Core MVC web application with Razor Views and controllers.

| Folder          | Description                                         |
|-----------------|-----------------------------------------------------|
| `Controllers/`  | MVC Controllers for each module                     |
| `Views/`        | Razor Views organized by feature                    |
| `wwwroot/`      | Static assets (CSS, JS, images, libraries)          |
| `Program.cs`    | Application entry point, DI configuration, pipeline |

---

## 🛠️ Tech Stack

| Technology                          | Purpose                          |
|-------------------------------------|----------------------------------|
| .NET 10                             | Runtime & SDK                    |
| ASP.NET Core MVC                    | Web framework                    |
| Entity Framework Core 10            | ORM & database access            |
| SQL Server                          | Relational database              |
| ASP.NET Core Identity               | Authentication & authorization   |
| AutoMapper 16                       | Object-to-object mapping         |
| Razor Views                         | Server-side rendering            |
| Bootstrap (via `lib/`)              | UI styling                       |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB or full instance)
- Visual Studio 2022+ or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/mohamedelbakry1/GymManagementSystem.git
   cd GymManagementSystem
   ```

2. **Configure the connection string**

   Update `appsettings.json` in the `GymManagementPL` project with your SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=GymManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Run the application**
   ```bash
   cd GymManagementPL
   dotnet run
   ```
   > The app will automatically apply pending migrations and seed initial data on first run.

4. **Open in browser**
   ```
   https://localhost:5001
   ```

---

## 📂 Project Structure

```
GymManagementSystem/
│
├── GymManagementDAL/                  # Data Access Layer
│   ├── Entities/
│   │   ├── BaseEntity.cs
│   │   ├── AppUser.cs
│   │   ├── GymUser.cs
│   │   ├── Member.cs
│   │   ├── Trainer.cs
│   │   ├── Session.cs
│   │   ├── Plan.cs
│   │   ├── Membership.cs
│   │   ├── Booking.cs
│   │   ├── Category.cs
│   │   ├── Address.cs
│   │   ├── HealthRecord.cs
│   │   └── Enums/
│   │       ├── Gender.cs
│   │       └── Specialties.cs
│   ├── Data/
│   │   ├── Contexts/
│   │   └── DataSeed/
│   └── Repositories/
│       ├── Interfaces/
│       └── Classes/
│
├── GymManagementBLL/                  # Business Logic Layer
│   ├── Services/
│   │   ├── Interfaces/
│   │   ├── Classes/
│   │   └── AttachmentService/
│   ├── ViewModels/
│   │   ├── AccountViewModels/
│   │   ├── MemberViewModels/
│   │   ├── TrainerViewModels/
│   │   ├── SessionViewModels/
│   │   ├── PlanViewModels/
│   │   ├── MembershipViewModels/
│   │   ├── BookingViewModels/
│   │   ├── UserViewModels/
│   │   └── AnalyticsViewModels/
│   └── MappingProfile.cs
│
├── GymManagementPL/                   # Presentation Layer
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── HomeController.cs
│   │   ├── MemberController.cs
│   │   ├── TrainerController.cs
│   │   ├── SessionController.cs
│   │   ├── PlanController.cs
│   │   ├── MembershipController.cs
│   │   ├── BookingController.cs
│   │   └── UserController.cs
│   ├── Views/
│   ├── wwwroot/
│   └── Program.cs
│
├── GymManagementSystem.slnx           # Solution file
└── README.md
```

---

## 🔑 Design Patterns Used

- **Repository Pattern** — Abstracts data access behind interfaces for testability and flexibility.
- **Unit of Work** — Coordinates multiple repository operations within a single transaction.
- **Service Layer Pattern** — Encapsulates business logic in dedicated service classes.
- **Dependency Injection** — All services and repositories are registered in the DI container.
- **ViewModel / DTO Pattern** — Separates domain models from presentation concerns.
- **AutoMapper Profiles** — Centralized mapping configuration between entities and ViewModels.

---

## 🤝 Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

## 📬 Contact

**Mohamed Elbakry** — [GitHub Profile](https://github.com/mohamedelbakry1)

---

> ⭐ If you found this project helpful, give it a star!