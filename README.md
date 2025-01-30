# 🛒 ECommerce API

ECommerce API is a **RESTful web service** built using **.NET 8** following **Clean Architecture** principles.  
It provides essential backend functionalities for an **e-commerce platform**, including:

- **User Authentication & Authorization** (JWT-based)
- **Product Management**
- **Cart & Order Processing**
- **Role-based Access Control (Admin & Users)**

## 🔍 Problem Statement

Small businesses and independent sellers often struggle with managing their online sales, inventory, and order tracking efficiently.  
Existing solutions are either too expensive or do not provide the necessary flexibility for businesses to scale.

### ✅ **Solution**

The **ECommerce API** is designed as a **scalable and cost-effective** backend solution that provides essential e-commerce functionalities such as:

- **User authentication & authorization** for secure transactions.
- **Product and inventory management** to keep stock updated.
- **Shopping cart and order management** to facilitate purchases.
- **Role-based access control** ensuring proper authorization for admins and customers.

## 🚀 Impact & Value

- 🕒 **Reduces manual order processing time** by up to **50%** through automation.
- 💰 **Provides a cost-effective backend** alternative to expensive e-commerce platforms.
- 🛍️ **Designed for scalability**—ideal for startups and small businesses aiming to transition online.
- 🔒 **Ensures secure transactions** through robust authentication and authorization mechanisms.

## 🛠️ Features

- **Secure Authentication**: Implements JWT-based authentication for secure user sessions.
- **Product Management**: CRUD operations for products with category associations.
- **Cart Functionality**: Allows users to add, update, and remove items from their cart.
- **Order Processing**: Facilitates order placement and status tracking.
- **Role-Based Access Control**: Differentiates access for Admin and regular users.

## 🚀 Getting Started

### 📥 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [PostgreSQL](https://www.postgresql.org/download/)

### 📦 Installation

1. **Clone the Repository**
   ```sh
   git clone https://github.com/SandeepBarla/ECommerceAPI.git
   cd ECommerceAPI
   ```

2. **Install Dependencies**
   ```sh
   dotnet restore
   ```

3. **Environment Variables Configuration**

    Update `appsettings.json` with the following settings:

    **Database Configuration**
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=5432;Database=ecommerce_db;Username=ecommerce_user;Password=your_password"
    }
    ```

    **JWT Authentication**
    ```json
    "Jwt": {
      "Key": "your_secret_key",
      "Issuer": "http://localhost:5000",
      "Audience": "http://localhost:5000"
    }
    ```

    ⚠️ **Replace placeholders (`your_password`, `your_secret_key`) with actual values.**  
    Use **environment variables** or a **secrets manager** for sensitive data.

4. **Apply Migrations and Update Database**
   ```sh
   dotnet ef database update --project ECommerceAPI.Infrastructure --startup-project ECommerceAPI.WebApi
   ```

5. **Build and Run the Application**
   ```sh
   dotnet build
   dotnet run --project ECommerceAPI.WebApi
   ```

   The API will be accessible at `https://localhost:5001` by default.


## 📋 API Endpoints

#### Auth

##### Login
```http
POST /api/auth/login
```
| Parameter  | Type     | Description              |
|-----------|---------|--------------------------|
| `email`   | `string` | **Required**. User email |
| `password`| `string` | **Required**. User password |

---

#### User

##### Register a new user
```http
POST /api/users
```
| Parameter    | Type     | Description               |
|-------------|---------|---------------------------|
| `name`      | `string` | **Required**. User name   |
| `email`     | `string` | **Required**. User email  |
| `password`  | `string` | **Required**. User password |

##### Get all users (Admin only)
```http
GET /api/users
```

##### Get user by ID
```http
GET /api/users/{userId}
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |

---

#### Cart

##### Get user's cart
```http
GET /api/users/{userId}/cart
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |

##### Add item to cart
```http
POST /api/users/{userId}/cart
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |
| `productId` | `string` | **Required**. Product ID  |
| `quantity` | `int`    | **Required**. Quantity     |

##### Remove item from cart
```http
DELETE /api/users/{userId}/cart
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |
| `productId` | `string` | **Required**. Product ID  |

---

#### Order

##### Place a new order
```http
POST /api/users/{userId}/orders
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |
| `cartId`  | `string` | **Required**. Cart ID       |

##### Get all orders for a user
```http
GET /api/users/{userId}/orders
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |

##### Get order details by ID
```http
GET /api/users/{userId}/orders/{id}
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |
| `id`      | `string` | **Required**. Order ID      |

##### Update order status (Admin only)
```http
PATCH /api/users/{userId}/orders/{id}
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `userId`  | `string` | **Required**. User ID       |
| `id`      | `string` | **Required**. Order ID      |
| `status`  | `string` | **Required**. New status    |

##### Get all orders (Admin only)
```http
GET /api/orders
```

---

#### Products

##### Add a new product (Admin only)
```http
POST /api/products
```
| Parameter  | Type     | Description                   |
|------------|---------|-------------------------------|
| `name`     | `string` | **Required**. Product name   |
| `price`    | `decimal`| **Required**. Product price  |
| `stock`    | `int`    | **Required**. Available stock|

##### Get all products
```http
GET /api/products
```

##### Get product by ID
```http
GET /api/products/{id}
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `id`      | `string` | **Required**. Product ID    |

##### Update product details (Admin only)
```http
PUT /api/products/{id}
```
| Parameter  | Type     | Description                   |
|------------|---------|-------------------------------|
| `id`       | `string` | **Required**. Product ID    |
| `name`     | `string` | **Required**. Product name  |
| `price`    | `decimal`| **Required**. Product price |
| `stock`    | `int`    | **Required**. Available stock|

##### Delete a product (Admin only)
```http
DELETE /api/products/{id}
```
| Parameter | Type     | Description                  |
|-----------|---------|------------------------------|
| `id`      | `string` | **Required**. Product ID    |


## 🧪 Running Tests

Execute the following command to run the test suite:

```sh
dotnet test
```



## 🎯 Future Improvements

- **Integrate Payment Gateway** for handling transactions securely.
- **Add WebSockets Support** for real-time notifications on order status.
- **Implement Advanced Filtering & Search** for improved product discovery.


## 🙌 Contributors

- **[Sandeep Barla](https://github.com/SandeepBarla)** - Full Stack Developer



