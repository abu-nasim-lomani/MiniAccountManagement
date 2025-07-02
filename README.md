# Mini Account Management System

A full-featured, professional mini accounting application developed using ASP.NET Core Razor Pages and a Stored-Procedure-Only database approach. This project demonstrates a robust architecture following SOLID principles and the Repository Pattern, featuring a secure, role-based user management system and a dynamic, interactive user interface.

## 
### https://mdabunasim-001-site1.qtempurl.com/
### Admin Login: admin@app.com        Password: Shumonbd1@
#
![Dashboard](/Screenshort's/Dashboard.png)
---

## ✨ Key Features

A quick overview of the main features implemented in this project.

| Feature                      | Description                                                                                                                                              | Status    |
| ---------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| **Chart of Accounts** | Full CRUD functionality with an interactive `jsTree` hierarchy view and automated account code generation.                                                 | ✅ Done    |
| **Voucher Entry** | Dynamic multi-line form supporting Journal, Payment, and Receipt vouchers with real-time balance validation.                                             | ✅ Done    |
| **Voucher List & Filtering** | A comprehensive list of all vouchers with the ability to filter transactions by a specific date range.                                             | ✅ Done    |
| **User & Role Management** | A powerful admin panel to manage users, roles, and permissions (Approve, Update Role, Change Password, Delete).                                                   | ✅ Done    |
| **Secure Registration** | A "First User is Admin" system with no hardcoded credentials. Subsequent users are registered with a "Viewer" role, pending Admin approval.        | ✅ Done    |
| **Dynamic Dashboard** | A smart dashboard showing real-time stats from the database (Total Accounts, Active Users, etc.).                                                        | ✅ Done    |
| **Excel Export** | _(Bonus Feature)_ Ability to export User Lists, Chart of Accounts, and filtered Voucher Lists directly to `.xlsx` files.                             | ✅ Done    |
| **Interactive UI** | Includes a separate public landing page, Toastr notifications for user feedback, and confirmation modals for critical actions.                             | ✅ Done    |

---

## 🛠️ Technology Stack

| Area          | Technology / Library                                       |
| ------------- | ---------------------------------------------------------- |
| **Backend** | ASP.NET Core (Razor Pages), .NET 8                         |
| **Database** | MS SQL Server                                              |
| **Data Access** | Dapper, ADO.NET, Repository Pattern, Stored Procedures Only |
| **Security** | ASP.NET Core Identity                                      |
| **Frontend** | Bootstrap 5, JavaScript, jQuery, jsTree, Toastr.js         |
| **Exporting** | ClosedXML                                                  |

---

## 🚀 Setup and Installation Guide

To run this project on your local machine, please follow these steps carefully.

### Step 1: Project Setup
1.  Git Clone or Download and extract the project `.zip` file to a local directory.

### Step 2: Configure the Database Connection
1.  In Visual Studio, find and open the `appsettings.json` file.
2.  Locate the `DefaultConnection` string and update the `Server` property to match your local SQL Server instance name (e.g., `.` or `(localdb)\mssqllocaldb`).
3.  Ensure the `Database` name is set to **`MiniAccountManagementDB`**.

### Step 3: Create the Blank Database
1.  Open **SQL Server Management Studio (SSMS)**.
2.  Right-click on the `Databases` folder and select `New Database...`.
3.  Set the **Database name** to **`MiniAccountManagementDB`** and click **OK**.

### Step 4: Run Initial Database Migrations
1.  Go back to **Visual Studio**.
2.  Open the **Package Manager Console** (`Tools > NuGet Package Manager > Package Manager Console`).
3.  Run the following command. This will create the necessary tables for user authentication and roles.
    ```powershell
    Update-Database -Context ApplicationDbContext
    ```

### Step 5: Execute the Master SQL Script
1.  In Visual Studio's **Solution Explorer**, find the master script file located at: `Data/Database/Master_Setup_Script.sql`.
2.  Open this file in **SQL Server Management Studio (SSMS)**.
3.  File > Open Fiile > Select `Data/Database/Master_Setup_Script.sql`. Then
4.  Click the **Execute** button. This single step will create all required application tables, types, and stored procedures.

### Step 6: Run the Application
You are all set! Press **F5** or click the "Run" button in Visual Studio to start the application.

### Step 7: Create the First Admin User
The application has no default users. The **first person to register** will automatically be assigned the **Admin** role and will be auto-approved.
-   Click on the "Register" link on the landing page and create your primary account.
-   Log in with your new account to get full admin access. You can then create other users from the **User Management** page.

---

## 📸 Screenshots

**Dashboard (for logged-in users)**
![Dashboard](/Screenshort's/Dashboard_Overview.png)

**Chart of Accounts (Interactive Tree View)**
*This page displays all financial accounts in a hierarchical folder-style view using jsTree.*
![Chart of Accounts](/Screenshort's/ChartOfAccount.png)

**User Management Panel**
*The admin panel for managing all users, their roles, and approval status.*
![User Management](/Screenshort's/User_Management.png)

**Voucher Entry Form**
*The form for entering transactions, featuring dynamic rows for multiple debit/credit entries.*
![Voucher Entry](/Screenshort's/Voucher_ENtry.png)

**Voucher List**
![Landing Page](/Screenshort's/Voucher_list.png)
