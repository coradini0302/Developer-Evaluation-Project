# 🚀 Developer Evaluation Project

## 📌 Overview

This project is a **.NET 8 Web API** built using **Clean Architecture, DDD principles, and CQRS**, designed to manage sales records with business rules applied at the domain level.

The application supports:

* Creating sales
* Retrieving sales (by ID and paginated list)
* Cancelling sales (logical deletion)
* Applying quantity-based discounts
* Structured logging and observability
* Full Dockerized environment (API + databases)

---

## 🧠 Business Rules

The system applies discount rules based on item quantity:

| Quantity      | Discount     |
| ------------- | ------------ |
| < 4 items     | No discount  |
| 4 – 9 items   | 10% discount |
| 10 – 20 items | 20% discount |
| > 20 items    | Not allowed  |

---

## 🏗️ Architecture

The project follows **Clean Architecture** with clear separation of concerns:

* **Domain** → Business rules and entities
* **Application** → Use cases (CQRS handlers)
* **Infrastructure (ORM)** → Database access (EF Core)
* **WebApi** → Controllers and HTTP layer
* **Common** → Shared utilities (pagination, etc.)

Additionally:

* **CQRS** is used to separate reads and writes
* **DDD** concepts are applied (entities, invariants, domain logic)
* **External Identities Pattern** is used for references between domains

---

## 📦 Tech Stack

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* MongoDB
* Redis
* MediatR (CQRS)
* Serilog (structured logging)
* xUnit + FluentAssertions (testing)
* Docker & Docker Compose

---

## 🔍 Features

* ✅ Create sale with items and automatic total calculation
* ✅ Quantity-based discount logic
* ✅ Prevent invalid operations (e.g., cancelling twice)
* ✅ Retrieve sale by ID (detailed view)
* ✅ Retrieve paginated sales list (optimized query)
* ✅ Logical cancellation of sales
* ✅ Structured logging with business events simulation
* ✅ Integration and unit tests
* ✅ Fully containerized environment

---

## 🧪 Running the Project with Docker

### 📦 Requirements

* Docker Desktop (or Docker Engine + Docker Compose plugin)

---

### ▶️ Start the Application

From the `template/backend` folder:

```bash
docker-compose up --build
```

---

### 🌐 Services & Ports

| Service    | URL / Port            | Description                     |
| ---------- | --------------------- | ------------------------------- |
| API        | http://localhost:8080 | REST API + Swagger (`/swagger`) |
| PostgreSQL | localhost:5432        | Main relational database        |
| MongoDB    | localhost:27017       | NoSQL database                  |
| Redis      | localhost:6379        | Cache / messaging support       |

---

### 🔐 Credentials

**PostgreSQL**

* Database: `developer_evaluation`
* Username: `postgres`
* Password: `postgres`

**MongoDB**

* Username: `developer`
* Password: `ev@luAt10n`

**Redis**

* Password: `ev@luAt10n`

---

## 🧠 Database Initialization

* Entity Framework Core migrations are **automatically applied on startup**
* No manual setup is required

---

## 🔄 Reset Environment

To clean and rebuild everything:

```bash
docker-compose down -v
docker-compose up --build
```

---

## 📡 API Usage

After starting the project, access Swagger:

👉 http://localhost:8080/swagger

You can:

* Create a sale
* Retrieve a sale by ID
* List sales with pagination
* Cancel a sale

---

## 🧾 Example Endpoints

```http
POST   /api/sales
GET    /api/sales/{id}
GET    /api/sales?page=1&pageSize=10
DELETE /api/sales/{id}/cancel
```

---

## 🧪 Running Tests

Run all tests with:

```bash
dotnet test
```

The project includes:

* Unit tests (domain logic)
* Integration tests (API behavior)

---

## 📊 Logging & Observability

The application uses **structured logging** with Serilog.

Logs include:

* Request lifecycle (HTTP)
* Database operations (EF Core)
* Business events simulation:

  * `SaleCreated`
  * `SaleCancelled`

Example:

```text
[INF] Event: SaleCreated | SaleId: xxx | Customer: John | TotalAmount: 850
```

---

## ⚠️ Notes

* The API runs over HTTP inside Docker (no HTTPS redirect)
* Services communicate using container names (not localhost)
* Data is persisted using Docker volumes

---

## 🎯 Summary

This project demonstrates:

* Clean Architecture and DDD best practices
* CQRS with MediatR
* Business rule enforcement at domain level
* Integration and unit testing
* Containerized environment with Docker Compose
* Observability through structured logging

---

## 💡 Author Notes

This project was designed to simulate a real-world backend system with production-ready patterns, focusing on scalability, maintainability, and clarity of business logic.




_______________________________________________________________________________________________________________________________________________________________________________________________________

Requires of project

# Developer Evaluation Project

`READ CAREFULLY`

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)

<!-- 
## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)
-->

## Project Structure
This section describes the overall structure and organization of the project files and directories. 

See [Project Structure](/.doc/project-structure.md)
