# 🛒 ECommerce API

ECommerce API is a backend service for an online shopping platform, designed using **.NET 8**, **Entity Framework Core**, and **PostgreSQL**. It provides essential functionalities such as **authentication**, **product management**, **cart handling**, **orders**, and **user management**.

## 🚀 Features

- **Authentication & Authorization** using **JWT Tokens**.
- **Product Management**: CRUD operations for products.
- **Cart Management**: Add, update, and remove items from the cart.
- **Order Processing**: Place orders and track their status.
- **User Management**: Fetch user profiles and order history.
- **Global Exception Handling** for improved error management.
- **Fluent Validation** for request validation.
- **Unit Tests** for controllers, services, validators, and repositories.

## 🛠 Refactoring

The project is currently undergoing **refactoring** to improve **code structure, maintainability, and testability**. The key improvements include:

- **Implementing Clean Architecture** by separating concerns into different layers.
- **Introducing Repository & Service Layers** for better abstraction.
- **Adding AutoMapper** to handle DTO-to-Entity transformations.
- **Implementing Global Exception Handling** for more robust error management.
- **Unit Testing** to ensure stability during refactoring.

## 📡 API Endpoints

### **Authentication**
- `POST /api/auth/register` - Register a new user.
- `POST /api/auth/login` - Authenticate and retrieve a JWT.
- `GET /api/auth/profile` - Get the authenticated user's profile.

### **Products**
- `POST /api/products` - Create a new product (Admin).
- `GET /api/products` - Retrieve all products.
- `GET /api/products/{id}` - Retrieve a specific product by ID.
- `PUT /api/products/{id}` - Update a product (Admin).
- `DELETE /api/products/{id}` - Delete a product (Admin).

### **Cart**
- `GET /api/cart` - Retrieve the user's cart.
- `POST /api/cart` - Add an item to the cart.
- `PATCH /api/cart` - Update cart items.
- `DELETE /api/cart` - Clear the cart.

### **Orders**
- `POST /api/orders` - Create a new order.
- `GET /api/orders` - Retrieve all orders.
- `GET /api/orders/{id}` - Retrieve order details by ID.
- `PUT /api/orders/{id}/status` - Update order status (Admin).
- `GET /api/orders/users/{userId}` - Retrieve orders by user.
- `GET /api/orders/all` - Retrieve all orders (Admin).

### **Users**
- `GET /api/users` - Retrieve all users (Admin).

## ⚡ Getting Started

### **1️⃣ Prerequisites**
Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### **2️⃣ Setup**
#### **Clone the repository**
```sh
git clone https://github.com/SandeepBarla/ECommerceAPI.git
cd ECommerceAPI
```
## 🛠 Environment Variables

Create a `.env` file in the root directory with the following variables:

```env
DB_CONNECTION_STRING=your_postgres_connection_string
JWT_SECRET=your_secret_key
```
## ✅ Unit Tests

This project includes unit tests for controllers, services, validators, and repositories. To run tests:

```sh
dotnet test
```

## 🎯 Author

- **Sandeep Barla** - [GitHub](https://github.com/SandeepBarla)