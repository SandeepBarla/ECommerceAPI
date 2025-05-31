# E-Commerce API Postman Collection

This directory contains Postman collection and environment files for testing the E-Commerce API.

## Files

- `ecommerce-api.postman_collection.json` - Complete API collection with all endpoints
- `environment.template.json` - Environment template with required variables

## Setup Instructions

### 1. Import Collection

1. Open Postman
2. Click "Import" button
3. Select `ecommerce-api.postman_collection.json`
4. The collection will be imported with all endpoints organized in folders

### 2. Setup Environment

1. Create a new environment in Postman
2. Set the following variables:
   - `ecommerceurl`: Your API base URL (e.g., `http://localhost:5000`)
   - `JWT_TOKEN`: Will be auto-populated by login requests

### 3. Authentication Flow

1. **Login First**: Run one of the login requests in the "Auth" folder
   - "Login User 1" for regular user testing
   - "Login Admin" for admin functionality testing
2. The login scripts automatically set the `JWT_TOKEN` variable
3. All other authenticated endpoints will use this token automatically

## Available Endpoints

### 🔐 Auth

- Login User 1/2
- Login Admin
- Google Login

### 👤 User Management

- Register User
- Get User by ID
- Get All Users (Admin)
- **Update User Profile** (NEW)

### 🏠 Address Management (NEW)

- Get User Addresses
- Get Address by ID
- Create Address
- Update Address
- Delete Address
- Set Default Address
- Get Default Address

### 📦 Products

- Create Product (Admin)
- Get All Products
- Get Product by ID
- Update Product (Admin)
- Delete Product (Admin)

### 🛒 Cart

- Get User Cart
- Add/Update Cart Items
- Delete Cart

### 📋 Orders

- Create Order
- Get User Orders
- Get Single Order
- Get All Orders (Admin)
- Update Order Status (Admin)

### 🏷️ Categories

- Get Categories
- Get Category by ID
- Create Category (Admin)
- Update Category (Admin)
- Delete Category (Admin)

### 📏 Sizes

- Get Sizes
- Get Size by ID
- Create Size (Admin)
- Update Size (Admin)
- Delete Size (Admin)

### ❤️ Favorites

- Mark as Favorite
- Unmark as Favorite
- Get User Favorites

## Testing Workflow

### For Address Management Testing:

1. **Login** as a user
2. **Create Address**: POST `/api/users/1/addresses`
3. **Get Addresses**: GET `/api/users/1/addresses`
4. **Update Address**: PUT `/api/users/1/addresses/1`
5. **Set Default**: PATCH `/api/users/1/addresses/1/set-default`
6. **Get Default**: GET `/api/users/1/addresses/default`
7. **Delete Address**: DELETE `/api/users/1/addresses/1`

### Sample Address Data:

```json
{
  "name": "Home",
  "street": "123 Main Street",
  "city": "New York",
  "state": "NY",
  "pincode": "123456",
  "phone": "1234567890",
  "isDefault": true
}
```

## Notes

- Replace `:userId` path parameters with actual user IDs (1, 2, etc.)
- Replace `:addressId` path parameters with actual address IDs
- Admin endpoints require admin authentication
- All authenticated endpoints automatically use the JWT token from login
- Phone numbers must be 10 digits, Pincode must be 6 digits

## Version Control

This collection is version controlled with your project. When you add new endpoints:

1. Update the collection in Postman
2. Export and replace `ecommerce-api.postman_collection.json`
3. Commit the changes to Git

This ensures your API documentation and tests stay in sync with your code!
