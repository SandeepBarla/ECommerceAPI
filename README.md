# 🛒 E-Commerce API - .NET 8 + PostgreSQL

### 🔗 **Live API (if deployed):** _Coming soon_

## 📌 Overview
This is a **.NET 8-based E-Commerce API** for managing users, orders, and products (coming soon).  
Currently, the project supports **JWT authentication, Order Management, and PostgreSQL integration**.

---

## 🚀 Features Implemented
✅ **User Authentication (JWT)**
- Register new users
- Login and get JWT tokens
- Get authenticated user profile

 ✅ **Order Management**
- Create new orders
- View orders for logged-in users
- Admins can update order status  

✅ **PostgreSQL Database Integration**

---

## 🚧 Features in Progress
🔜 **Product Management API (CRUD for products)**  
🔜 **Secure Payments (Stripe/Razorpay)**  
🔜 **Role-Based Access (Admin & Customer)**  
🔜 **Swagger API Documentation**  
🔜 **Docker & Cloud Deployment (Azure/AWS)**

---

## 🛠️ Tech Stack
- **Backend:** `.NET 8 Web API`
- **Database:** `PostgreSQL`
- **Authentication:** `JWT + BCrypt`
- **API Documentation:** `Swagger (Coming Soon)`
- **Deployment:** `Docker, Azure/AWS (Coming Soon)`

---

## 🔧 Installation & Setup
### **Clone the Repository**
```sh
git clone https://github.com/SandeepBarla/ECommerceAPI.git
cd ECommerceAPI
```
### **Install Dependencies**
```sh
dotnet restore
```
### **Update `appsettings.json`**
Edit `appsettings.json` to configure **database & JWT**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ecommerce_db;Username=yourusername;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your_secret_key_here",
    "Issuer": "yourdomain.com",
    "Audience": "yourdomain.com"
  }
}
```
### **Apply Migrations**
```sh
dotnet ef database update
```
### **Run the Application**
```sh
dotnet run
```
> **API will be available at:** `http://localhost:5000/swagger/index.html`

---

## 🔗 API Endpoints

### **User Authentication**
| Method | Endpoint | Description |
|--------|---------|------------|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Login and get JWT token |
| `GET`  | `/api/auth/profile` | Get user details (JWT required) |

### **Order Management**
| Method | Endpoint | Description |
|--------|---------|------------|
| `POST` | `/api/orders` | Create an order |
| `GET`  | `/api/orders` | View all orders for logged-in user |
| `GET`  | `/api/orders/{id}` | Get order details |
| `PUT`  | `/api/orders/{id}/status` | Admin: Update order status |

---

## 🚀 Deployment (Coming Soon)
This project will be deployed to **Azure App Service / AWS** with **Docker support**.

---

## 🙌 Contributors
- **[Sandeep Barla](https://github.com/SandeepBarla)** - Full Stack Developer

---

## 📜 License
This project is **open-source** under the [MIT License](LICENSE).