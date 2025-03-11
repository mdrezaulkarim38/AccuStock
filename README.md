# AccuStock

**AccuStock** is a powerful and user-friendly **Accounting and Inventory Management System** built with **ASP.NET Core MVC**, **Entity Framework Core**, **Dapper**, and **MS SQL Server**. It provides seamless financial and inventory management features, making it an ideal solution for businesses of all sizes.

## 🚀 Features

### **Accounting Features**
- 📒 **Ledger & Journal Entries** – Maintain accurate financial records.
- 📊 **Financial Reporting** – Generate balance sheets, income statements, and cash flow reports.
- 🔀 **Multi-User Role-Based Access** – Secure and manage user permissions efficiently.
- 🔄 **Subscription-Based Access Control** – Flexible access based on user subscriptions.

### **Inventory Management**
- 📦 **Advanced Inventory Tracking** – Monitor stock levels in real time.
- 📊 **Stock Movement Analysis** – Track product inflow and outflow.
- 🔔 **Low Stock Alerts** – Get notified when stock levels are low.
- 📑 **Batch & Serial Number Tracking** – Ensure product traceability.

## 🛠️ Tech Stack
- **Framework:** ASP.NET Core MVC
- **Database:** MS SQL Server
- **ORMs:** Entity Framework Core, Dapper
- **Frontend:** Razor Views, Bootstrap
- **Authentication:** Role-based & Subscription-based Access Control

## 📂 Project Structure
```
AccuStock/
├── AccuStock.csproj
├── AccuStock.sln
├── appsettings.json
├── appsettings.Development.json
├── bin/
├── Controllers/
├── Data/
├── Interface/
├── Migrations/
├── Models/
├── obj/
├── Program.cs
├── Properties/
├── README.md
├── Services/
├── Views/
└── wwwroot/
```

## 🛠️ Installation & Setup
### **Prerequisites**
- .NET 8 or later installed
- MS SQL Server

### **Step 1: Clone the Repository**
```sh
git clone https://github.com/mdrezaulkarim38/AccuStock.git
cd AccuStock
```

### **Step 2: Configure the Database**
- Update the `appsettings.json` file with your database connection string.
- Run the following command to apply migrations:
  ```sh
  dotnet ef database update
  ```

### **Step 3: Run the Application**
```sh
dotnet run
```

## 📜 License
This project is licensed under the [MIT License](LICENSE).

---
💡 **AccuStock** – Empowering Businesses with Smart Inventory & Accounting Solutions!

