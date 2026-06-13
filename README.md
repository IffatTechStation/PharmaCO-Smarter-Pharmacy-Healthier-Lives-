# PharmaCO – Smarter Pharmacy, Healthier Lives

PharmaCO is a full‑stack web application designed to modernize pharmacy and healthcare management. It provides a robust platform for handling prescriptions, orders, inventory, and patient records with secure authentication and role‑based access.

## 🚀 Features
- 🔐 JWT authentication with role‑based access
- 📦 Order management (CRUD operations)
- 💊 Pharmacy inventory tracking
- 👩‍⚕️ Patient and prescription management
- 📊 Dashboard with analytics
- ⚡ Built using ASP.NET Core API + EF Core
- 🧪 API tested with Postman

## 🛠️ Tech Stack
- **Backend:** ASP.NET Core, Entity Framework Core, SQL Server
- **Frontend:** HTML, CSS, JavaScript (minimal UI for auth & dashboard)
- **Database:** SQL Server
- **Tools:** Postman for API testing, GitHub for version control

## 📂 Project Structure
PharmaCO/
├── Controllers/        # API controllers
├── Models/             # Entity models
├── Repositories/       # Repository pattern
├── Services/           # Business logic
├── wwwroot/            # Static frontend files
├── appsettings.json    # Configuration
└── Program.cs          # Entry point


## ⚙️ Setup Instructions
1. Clone the repository:
   ```bash
   git clone https://github.com/IffatTechStation/PharmaCO-Smarter-Pharmacy-Healthier-Lives-.git
2. Configure appsettings.json with your SQL Server connection string.

3. Run migrations:

bash
dotnet ef database update

4. Start the API:

bash
dotnet run

📜 License
This project is licensed under the MIT License.

✨ PharmaCO – making pharmacy smarter and healthcare healthier.
