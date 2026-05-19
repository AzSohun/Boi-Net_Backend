# Boi.Net Backend API

A modern ASP.NET Core 10 Web API for a comprehensive online bookstore e-commerce platform. Boi.Net provides robust backend services for managing books, user authentication, orders, payments, and more with enterprise-grade architecture and best practices.

## 📖 Table of Contents

- [🌟 Features](#-features)
- [🛠️ Tech Stack](#️-tech-stack)
- [📋 Prerequisites](#-prerequisites)
- [🚀 Getting Started](#-getting-started)
- [⚙️ Configuration](#️-configuration)
- [📚 API Endpoints](#-api-endpoints)
- [🔐 Authentication](#-authentication)
- [🗄️ Database Architecture](#️-database-architecture)
- [🏗️ Project Structure](#️-project-structure)
- [🐳 Docker Deployment](#-docker-deployment)
- [📝 Development Guide](#-development-guide)
- [🤝 Contributing](#-contributing)

## 🌟 Features

### Core Features
- ✅ **Book Catalog Management** - Complete CRUD operations with advanced search and filtering
- ✅ **User Authentication & Authorization** - JWT-based security with role-based access control
- ✅ **Shopping Cart & Orders** - Full order lifecycle management from creation to fulfillment
- ✅ **Payment Processing** - Stripe integration for secure, PCI-compliant transactions
- ✅ **Image Management** - Cloudinary integration for efficient book cover uploads and optimization
- ✅ **Caching Layer** - Redis caching for improved API performance and reduced database load
- ✅ **API Documentation** - Interactive OpenAPI/Swagger documentation with Scalar UI
- ✅ **Global Exception Handling** - Centralized error handling with standardized responses
- ✅ **CORS Support** - Pre-configured for frontend integration with proper security headers

### Additional Features
- 🔒 Password hashing with BCrypt
- 📊 Comprehensive logging and monitoring capabilities
- 🌐 Multi-environment configuration (Development, Production)
- 🐳 Docker containerization for easy deployment
- 📱 RESTful API design patterns
- 🔄 Automatic data mapping with AutoMapper

## 🛠️ Tech Stack

| Category | Technologies |
|----------|---------------|
| **Framework** | ASP.NET Core 10, C# |
| **Database** | PostgreSQL with Entity Framework Core 10.0.7 |
| **Authentication** | JWT Bearer Tokens, ASP.NET Identity |
| **API Documentation** | Scalar.AspNetCore 2.14.9, OpenAPI |
| **Payment Processing** | Stripe.net 51.1.0 |
| **Image Management** | CloudinaryDotNet 1.29.0 |
| **Caching** | Redis via StackExchange.Redis 2.8.0 |
| **Object Mapping** | AutoMapper 16.1.1 |
| **Security** | BCrypt.Net-Next 4.1.0 |
| **Containerization** | Docker, Multi-stage builds |

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **PostgreSQL** - Version 12+ (local or remote instance)
- **Redis** - Version 6.0+ (for caching layer)
- **Git** - For version control
- **Docker & Docker Compose** (optional, for containerized setup)

### External Accounts Required
- **Cloudinary Account** - For image hosting (free tier available at [cloudinary.com](https://cloudinary.com))
- **Stripe Account** - For payment processing (test/live keys from [stripe.com](https://stripe.com))

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/Boi.Net.git
cd Boi.Net
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Environment Variables

Create or update the `appsettings.json` file in the `Boi.Net` directory with your configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=boinetdb;Username=postgres;Password=yourpassword",
    "RedisConnection": "your-redis-endpoint:6379,Password=your-password"
  },
  "CloudinarySettings": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  },
  "Jwt": {
    "Key": "YourVeryLongSecretKeyThatIsAtLeast32BytesLongForSecurity",
    "Issuer": "BoiNetApp",
    "Audience": "BoiNetUsers"
  },
  "Stripe": {
    "WebhookSecret": "whsec_your_webhook_secret",
    "SecretKey": "sk_test_your_secret_key"
  }
}
```

### 4. Create PostgreSQL Database

```bash
# Using psql command line
psql -U postgres
CREATE DATABASE boinetdb;
```

### 5. Apply Database Migrations

```bash
dotnet ef database update
```

### 6. Run the Application

#### Development Mode
```bash
dotnet run
```

#### Watch Mode (Auto-reload on file changes)
```bash
dotnet watch run
```

The API will be available at:
- **HTTP**: `http://localhost:5298`
- **HTTPS**: `https://localhost:7293`

API documentation: `https://localhost:7293/scalar/v1`

## ⚙️ Configuration

### Environment Settings

#### Development (`appsettings.Development.json`)
- Debug logging enabled
- Database migrations auto-applied
- CORS allows localhost:3000
- Detailed error responses

#### Production (`appsettings.json`)
- Minimal logging
- External database connection
- CORS restricted to frontend domain
- Generic error responses for security

### Key Configuration Parameters

| Parameter | Purpose | Example |
|-----------|---------|---------|
| `ConnectionStrings.DefaultConnection` | PostgreSQL connection | `Host=localhost;Port=5432;Database=boinetdb;Username=postgres;Password=pass` |
| `ConnectionStrings.RedisConnection` | Redis cache endpoint | `localhost:6379,Password=redispass` |
| `Jwt.Key` | JWT signing key (min 32 bytes) | `YourVeryLongSecureKeyHere...` |
| `Jwt.Issuer` | Token issuer | `BoiNetApp` |
| `Jwt.Audience` | Token audience | `BoiNetUsers` |
| `Stripe.SecretKey` | Stripe API key | `sk_test_xxxxx` |
| `Cloudinary.CloudName` | Cloudinary account name | `your-cloud-name` |

## 📚 API Endpoints

### Base URL
- Development: `https://localhost:7293/api`
- Production: `https://yourdomain.com/api`

### Authentication Endpoints (`/auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---|
| `POST` | `/auth/register` | Register a new user account | ❌ |
| `POST` | `/auth/login` | Login and receive JWT token | ❌ |

**Register Request Body:**
```json
{
  "email": "user@example.com",
  "username": "username",
  "password": "SecurePassword123"
}
```

**Login Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123"
}
```

**Response:**
```json
{
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "user-id-here"
}
```

**Super Admin Credential:**
```json
{
  "email": "admin@email.com",
  "password": "Admin@pass123"
}
```

### Book Endpoints (`/book`)

| Method | Endpoint | Description | Auth Required | Admin Only |
|--------|----------|-------------|---|---|
| `GET` | `/book/all-books` | Get all books with pagination and filtering | ❌ | ❌ |
| `GET` | `/book/{id}` | Get book details by ID | ❌ | ❌ |
| `POST` | `/book` | Create a new book | ✅ | ✅ |
| `PUT` | `/book/{id}` | Update book information | ✅ | ✅ |
| `DELETE` | `/book/{id}` | Delete a book | ✅ | ✅ |

**Query Parameters for `/book/all-books`:**
- `search` - Search by title or author name
- `genre` - Filter by book genre
- `author` - Filter by author name
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

### Order Endpoints (`/order`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---|
| `GET` | `/order` | Get all user's orders | ✅ |
| `GET` | `/order/{id}` | Get order details by ID | ✅ |
| `POST` | `/order` | Create a new order | ✅ |
| `PUT` | `/order/{id}` | Update order status | ✅ |
| `DELETE` | `/order/{id}` | Cancel an order | ✅ |

**Create Order Request Body:**
```json
{
  "items": [
    {
      "bookId": "book-id-here",
      "quantity": 2,
      "price": 29.99
    }
  ],
  "shippingAddress": "123 Main St, City, State, ZIP"
}
```

### Payment Endpoints (`/payment`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---|
| `POST` | `/payment/create-payment-intent` | Create Stripe payment intent | ✅ |
| `POST` | `/payment/webhook` | Stripe webhook handler | ❌ |

### User Endpoints (`/user`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---|
| `GET` | `/user/profile` | Get current user profile | ✅ |
| `PUT` | `/user/profile` | Update user profile information | ✅ |
| `PUT` | `/user/admin/update-user/{id}` | Admin: Update any user (Admin only) | ✅ |
| `DELETE` | `/user/{id}` | Delete user account | ✅ |

## 🔐 Authentication

### JWT Token Usage

Include the JWT token in the `Authorization` header for authenticated requests:

```bash
curl -X GET "https://localhost:7293/api/user/profile" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Password Requirements

All user passwords must meet the following criteria:

| Requirement | Details |
|-------------|---------|
| **Minimum Length** | 8 characters |
| **Digits** | At least one digit (0-9) |
| **Lowercase** | At least one lowercase letter (a-z) |
| **Unique Email** | Email address must be unique across the system |

**Valid Example:** `MyPass123`  
**Invalid Examples:** `short1`, `NoDigits`, `alllowercase123`

### User Roles

- **User** - Standard user with order and profile management access
- **Admin** - Administrative access to book management and user administration

## 🗄️ Database Architecture

### Entity Models

#### User Model
- Extends `IdentityUser` for authentication
- Fields: Email, Username, PhoneNumber, Address, UserRole
- Relationships: One-to-Many with Orders

#### Book Model
- Fields: Title, Author, Description, Genre, ISBN, Price, CoverImageUrl, Stock
- Decimal precision: 18,2 (prevents floating-point errors)
- Relationships: One-to-Many with OrderItems

#### Order Model
- Fields: UserId, OrderDate, Status, TotalAmount, ShippingAddress
- Decimal precision: 18,2
- Relationships: One-to-Many with OrderItems, Many-to-One with User

#### OrderItem Model
- Fields: OrderId, BookId, Quantity, Price
- Decimal precision: 18,2
- Relationships: Many-to-One with Order and Book

### Database Diagram (Conceptual)

```
User (1) ──────── (N) Order
           └─── (1) ──────── (N) OrderItem ──── (N) Book
```

### Migrations

Current migration: `20260518170619_InitialPostgresCreate`
- Creates all base tables
- Sets up Identity tables for authentication
- Establishes relationships and constraints

## 🏗️ Project Structure

```
Boi.Net/
├── Controllers/              # API endpoint handlers
│   ├── AuthController.cs
│   ├── BookController.cs
│   ├── OrderController.cs
│   ├── PaymentController.cs
│   └── UserController.cs
├── Services/                 # Business logic layer
│   ├── AuthService.cs
│   ├── BookService.cs
│   ├── OrderService.cs
│   ├── PaymentService.cs
│   ├── UserService.cs
│   ├── IPhotoService.cs
│   └── PhotoService.cs
├── Models/                   # Database entity models
│   ├── User.cs
│   ├── Book.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── DTOs/                     # Data Transfer Objects
│   ├── AuthDTOs/
│   ├── BookDTOs/
│   ├── OrderDTOs/
│   └── UserDTOs/
├── Data/                     # Database context
│   └── BoiNetDbContext.cs
├── Mappers/                  # AutoMapper profiles
│   └── MappingProfile.cs
├── Migrations/               # EF Core migrations
├── Exceptions/               # Custom exception handlers
│   └── GlobalExceptionHandler.cs
├── Settings/                 # Configuration classes
│   └── CloudinarySettings.cs
├── Program.cs                # Application entry point
├── appsettings.json          # Configuration file
├── Boi.Net.csproj            # Project file
└── Properties/
    └── launchSettings.json   # Launch profiles
```

## 🐳 Docker Deployment

### Build Docker Image

```bash
docker build -t boi-net:latest .
```

### Run Container

```bash
docker run -d \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=boinetdb;Username=postgres;Password=yourpassword" \
  -e Jwt__Key="YourSuperSecretKeyHere" \
  -e Stripe__SecretKey="sk_test_xxxxx" \
  -e CloudinarySettings__CloudName="your-cloud-name" \
  --name boi-net-api \
  boi-net:latest
```

### Docker Compose (Production)

For full stack deployment with PostgreSQL and Redis:

```bash
docker-compose up -d
```

## 📝 Development Guide

### Adding a New Feature

1. **Create Model** - Define entity in `Models/`
2. **Create DTO** - Define data transfer object in `DTOs/`
3. **Create Service** - Implement business logic in `Services/`
4. **Create Controller** - Add endpoints in `Controllers/`
5. **Update Mapping** - Add AutoMapper configuration in `Mappers/MappingProfile.cs`
6. **Add Migration** - Run `dotnet ef migrations add FeatureName`
7. **Update Database** - Run `dotnet ef database update`

### Running Tests

```bash
dotnet test
```

### Code Standards

- Follow C# naming conventions (PascalCase for classes, camelCase for properties)
- Use async/await for I/O operations
- Add XML documentation comments for public methods
- Implement error handling with try-catch blocks
- Use dependency injection for services

### Debugging

Enable detailed logging in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

## 🤝 Contributing

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Review Process

- All pull requests require at least one approval
- Ensure all tests pass
- Follow the project's code style guidelines
- Update documentation as needed

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙋 Support

For issues and questions:
- Create an issue on the GitHub repository
- Check existing issues for solutions
- Contact the development team

---

**Last Updated:** May 19, 2026  
**Version:** 1.0.0  
**Maintained by:** Boi.Net Development Team
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

