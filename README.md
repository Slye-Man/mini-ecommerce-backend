# Mini E-Commerce Platform - CSRF Vulnerability Demonstration
Educational Purpose Only
This project is an intentionally vulnerable e-commerce application designed to demonstrate CSRF (Cross-Site Request Forgery) attacks and their impact. **DO NOT** deploy to production or use with real user data.

# Project Overview
A full-stack e-commerce application built with ASP.NET Core (C#) backend and React frontend, specifically designed with CSRF vulnerabilities for security research and educational purposes.

## Technology Stack
### Backend

- Framework: ASP.NET Core Web API
- Database: SQLite with Entity Framework Core
- Authentication: Cookie-based session management (intentionally vulnerable)
- Password Hashing: BCrypt.Net

### Frontend

- Framework: React
- HTTP Client: Fetch API
- Routing: React Router DOM

### Phase 1-3: Authentication

### User Registration

- Email and username validation
- BCrypt password hashing
- Automatic cart creation for new users
- Input validation with data annotations

### User Login

- Username/email authentication
- Session cookie creation (HttpOnly, SameSite: Lax)
- Password verification with BCrypt
- Session management

### Cookie Authentication Middleware

- Automatic cookie validation on requests
- User context attachment to HTTP requests
- No CSRF token validation (intentional vulnerability)

### Phase 4: CSRF Vulnerability Implementation

- Cookie-based authentication without CSRF tokens
- Permissive CORS configuration allowing credentials
- No Origin/Referer header validation
- All state-changing operations rely solely on cookies

### Phase 5: User Management
Endpoints:

- GET /api/user/profile - Get user profile
- PUT /api/user/profile - Update profile information
- PUT /api/user/change-password - Change password
- DELETE /api/user/account - Delete account

### Phase 6: Product Management
Endpoints (Public):

GET /api/product - Get all products with filtering

Supports: category, price range, search

- GET /api/product/{id} - Get product by ID
- GET /api/product/category/{category} - Get products by category
- GET /api/product/categories - Get all categories

### Product Database:
30 seeded products across 5 categories:

- Electronics (8 products)
- Clothing (6 products)
- Home & Kitchen (6 products)
- Books (3 products)
- Sports & Outdoors (7 products)

### Setup Instructions
Backend Setup

Clone the repository

```
git clone <repository-url>
cd mini-e-commerce
```

Install dependencies
```
dotnet restore
```
Apply database migrations

```
dotnet ef database update
```

Run the application
```
dotnet run
```
Backend will run on `http://localhost:5129`

# Disclaimer
This software is provided for educational purposes only. The authors are not responsible for any misuse or damage caused by this software. Use at your own risk in controlled environments only.
