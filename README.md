# 🛍️ Online Shopping Platform

## 📄 Step 1: Review the Architecture & Design Document

One of the key requirements of this project was to create a document explaining the **application architecture**, **security**, **component interactions**, and other essential implementation decisions.

You can view the full document here with full explanation of all :  
🔗 [Architecture & Design Document (Google Drive)](https://drive.google.com/file/d/1Dpx7RDgYAEMMBgLeMZJFLRMloQTBcNTm/view?usp=sharing)

## ✅ Functionality Overview

Alongside the design document, the task was to deliver a minimal implementation of a **web shopping cart system**. The following core features were implemented:

- 🔄 **Load products** from the backend
- ➕ **Add products to cart**
- ➖ **Remove products from cart**
- 🛒 **Place an order**
- 🔐 **OAuth login support** for users
- 🧠 **Cart merge functionality** between anonymous and logged-in sessions
- 🛡️ Focus on **security**:
  - Authentication & Authorization
  - ID **encryption/decryption** between frontend and backend to prevent predictable and visible identifiers
- 💓 **Health check endpoint** to verify database connectivity (PoC-level)
- 📁 **Serilog-based file logging** for request tracking and debugging

> **Note:** As a bonus, a very simple frontend in Angular was implemented. It serves as a basic UI needs and would need more work to be fully functional.

## ⚙️ Prerequisites for building the project
 
Make sure the following tools are installed on your machine:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js and npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) with **ASP.NET and web development** workload

---

## 🚀 How to Run the Application

### 🧩 Step 1: Build the Angular Frontend

Navigate to the frontend directory and build the project:

```bash
cd ShoppingWeb
npm install
ng build
```

### 🧪 Step 2: Run the .NET Backend

1. Open the solution file `OnlineShoppingPlatform.sln` in **Visual Studio**.
2. In the Visual Studio toolbar, select the **IIS Express** run profile.
3. Click the green **▶ Run** button.

Visual Studio will:

- Build the backend
- Serve the Angular frontend (from the `dist/` folder created by `ng build`)
- Launch your default browser with the application running