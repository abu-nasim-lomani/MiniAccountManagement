# Mini Account Management System

A full-featured, professional mini accounting application built with ASP.NET Core Razor Pages and a Stored-Procedure-Only database approach. This project demonstrates a robust architecture following SOLID principles and the Repository Pattern, featuring a secure, role-based user management system and a dynamic, interactive user interface.

---

## 🔑 Key Features

### Accounting Core
-   **Chart of Accounts (COA):** Full CRUD functionality with a dynamic, interactive `jsTree` hierarchy view.
-   **Automated Account Codes:** Intelligent, automatic code generation for both top-level and child accounts directly from the database.
-   **Voucher Entry:** A dynamic multi-line form supporting Journal, Payment, and Receipt vouchers with real-time debit/credit total calculations.
-   **Voucher List & Filtering:** A comprehensive list of all vouchers with the ability to filter transactions by a date range.

### Administration & Security
-   **Role-Based Access Control:** Three distinct user roles (**Admin**, **Accountant**, **Viewer**) with granular permissions for pages and actions.
-   **User Management Panel:** A powerful dashboard for Admins to manage all system users (Create, Approve, Update Role, Change Password, Delete).
-   **Secure Registration:** A "First User is Admin" system with no hardcoded credentials in the source code.
-   **Immediate Permission Updates:** Uses ASP.NET Core Identity's Security Stamp feature to ensure permission changes are reflected instantly.

### UI/UX & Bonus Features
-   **Dynamic Dashboard:** A smart dashboard showing real-time stats like Total Accounts, Active Users, and Today's Transactions.
-   **Public Landing Page:** A separate, professional landing page for unauthenticated visitors.
-   **Excel Export:** Ability to export User Lists, Chart of Accounts, and filtered Voucher Lists directly to `.xlsx` files.
-   **Toast Notifications:** Interactive notifications for user feedback on successful or failed actions.

---

## 🛠️ Technology Stack

-   **Backend:** ASP.NET Core (Razor Pages)
-   **Database:** MS SQL Server (Stored Procedures only for data manipulation)
-   **Data Access:** Dapper, ADO.NET, Repository Pattern
-   **Authentication & Authorization:** ASP.NET Core Identity
-   **Frontend:** Bootstrap 5, JavaScript, jQuery, jsTree, Toastr.js
-   **Excel Export:** ClosedXML

---

## 🚀 Setup and Installation

To run this project locally, please follow these steps:

1.  **Prerequisites:**
    * .NET 8 SDK (or newer)
    * Microsoft SQL Server (2017 or newer)

2.  **Clone the Repository:**
    ```bash
    git clone [Your_GitHub_Repo_URL]
    ```

3.  **Configure Database:**
    * Open the project in Visual Studio 2022.
    * In `appsettings.json`, update the `DefaultConnection` string with your local SQL Server details.
    * In SQL Server Management Studio (SSMS), create a new blank database with the same name used in your connection string (e.g., `MiniAccountManagementDB`).

4.  **Run Database Scripts (2 Steps):**

    * **Step 4.1: Identity Tables**
        In Visual Studio's **Package Manager Console**, run the following command to automatically create the ASP.NET Identity tables:
        ```powershell
        Update-Database -Context ApplicationDbContext
        ```

    * **Step 4.2: Application Tables & Logic**
        Now, to create all other tables (ChartOfAccounts, Vouchers) and stored procedures at once, simply execute the master script.
        -   In the project's Solution Explorer, find the script at: `/Database/Master_Setup_Script.sql`.
        -   Open this file in **SQL Server Management Studio (SSMS)**.
        -   Ensure your new blank database is selected.
        -   Click the **Execute** button. This single step will create all necessary tables and stored procedures.

5.  **Run the Application:**
    * Press **F5** or click the "Run" button in Visual Studio.

6.  **Create the First Admin User:**
    * The application has no default users. The **first person to register** will automatically be assigned the **Admin** role.
    * Click on the "Register" link on the landing page and create your primary account.
    * Log in with your new account to get full admin access. You can now create other users (`Accountant`, `Viewer`) from the User Management page.

---

## 📸 Screenshots

*(এই সেকশনে আপনার অ্যাপ্লিকেশনের কয়েকটি সুন্দর স্ক্রিনশট যোগ করুন। যেমন:)*
* *(Screenshot of the Login Page)*
* *(Screenshot of the Dashboard Home Page with dynamic stats)*
* *(Screenshot of the Chart of Accounts tree view)*
* *(Screenshot of the User Management page showing roles and actions)*
* *(Screenshot of the Voucher Entry form with multiple lines)*
