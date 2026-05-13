# Boi.Net Backend

A modern ASP.NET Web API for an online bookstore platform built with .NET 10. This project provides comprehensive backend services for managing books, user authentication, orders, and payments.

## 🌟 Features

- **Book Management**: Complete CRUD operations for books with search, filtering by genre and author
- **User Authentication & Authorization**: JWT-based authentication with Identity Framework
- **Payment Integration**: Stripe integration for secure payment processing
- **Order Management**: Full order lifecycle management
- **Image Hosting**: Cloudinary integration for book cover uploads
- **Caching**: Redis caching for improved performance
- **API Documentation**: OpenAPI/Swagger integration with Scalar
- **Global Exception Handling**: Centralized error handling middleware
- **CORS Support**: Configured for frontend integration

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10
- **Database**: SQL Server with Entity Framework Core 10.0.7
- **Authentication**: JWT Bearer tokens with ASP.NET Identity
- **API Documentation**: Scalar.AspNetCore 2.14.9
- **Payment Processing**: Stripe.net 51.1.0
- **Image Management**: CloudinaryDotNet 1.29.0
- **Caching**: Redis via StackExchangeRedis
- **Object Mapping**: AutoMapper 16.1.1
- **Password Hashing**: BCrypt.Net-Next 4.1.0

## 📋 Prerequisites

- .NET 10 SDK
- SQL Server (local or remote)
- Redis server (for caching)
- Cloudinary account (for image hosting)
- Stripe account (for payment processing)

## 🚀 Getting Started

### 1. Clone the Repository

### 2. Configure Environment Variables

Update `appsettings.json` with your configuration:

### 3. Apply Database Migrations

### 4. Run the Application

The API will be available at `https://localhost:7000` (or your configured port).

## 📚 API Endpoints

### Books
- `GET /api/book/all-books` - Get all books with filtering options
  - Query Parameters: `search`, `genre`, `author`
- `POST /api/book` - Create a new book (Admin only)
- `PUT /api/book/{id}` - Update a book (Admin only)
- `DELETE /api/book/{id}` - Delete a book (Admin only)

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

### Orders
- `GET /api/order` - Get user orders
- `POST /api/order` - Create a new order
- `GET /api/order/{id}` - Get order details

### Payments
- `POST /api/payment/create-payment-intent` - Create Stripe payment intent
- `POST /api/payment/webhook` - Handle Stripe webhooks

### Users
- `GET /api/user/profile` - Get user profile (Authorized)
- `PUT /api/user/profile` - Update user profile (Authorized)

## 🔐 Authentication

The API uses JWT (JSON Web Tokens) for authentication. Include the token in the Authorization header:

### Password Requirements
- Minimum length: 8 characters
- Must contain at least one digit
- Must contain at least one lowercase letter
- Email must be unique

## 🗄️ Database Models

- **User**: User account information (extends IdentityUser)
- **Book**: Book catalog with metadata
- **Order**: User orders with items
- **Payment**: Payment transaction records

## 🔄 Services Architecture

- **BookService**: Book management and search operations
- **AuthService**: User authentication and token generation
- **PaymentService**: Stripe payment processing
- **OrderService**: Order management
- **UserService**: User profile management
- **PhotoService**: Image upload and management via Cloudinary

## ⚙️ Configuration

### JWT Settings
Configure JWT parameters in `appsettings.json`:
### CORS Policy
Frontend origin is configured to `http://localhost:5173`. Modify in `Program.cs` if needed.

### Redis Caching
Redis is configured with instance name `BoiNet_` for cache isolation.

## 🛡️ Security Features

- **Password Hashing**: BCrypt for secure password storage
- **JWT Validation**: Token validation with issuer and audience checks
- **CORS Configuration**: Restricted to frontend domain
- **Global Exception Handling**: Prevents sensitive information leakage
- **Clock Skew**: Set to zero for strict token expiration

## 📝 Logging

Logging is configured in `appsettings.json`:
- Default level: Information
- Microsoft.AspNetCore: Warning

## 🧪 Error Handling

The API implements global exception handling with `GlobalExceptionHandler` middleware that returns standardized error responses.

## 🚢 Deployment

1. Ensure all environment variables are set correctly
2. Run database migrations: `dotnet ef database update`
3. Build the project: `dotnet build`
4. Publish: `dotnet publish -c Release`

## 📞 Support & Contribution

For issues and contributions, visit the [GitHub Repository](https://github.com/AzSohun/Boi-Net_Backend).

## 📄 License

This project is part of the Boi.Net platform.

---

**Built with ❤️ using ASP.NET Core 10**

