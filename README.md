# HotelStay Booking Platform Backend API

An enterprise hotel search and booking backend built with **ASP.NET Core 10 Minimal APIs**, leveraging Clean Architecture, the Provider Pattern, Fluent Validation, Dependency Injection, and SOLID design principles.

---

## 🏛️ Architectural Overview & Design Patterns

The backend is structured to separate concerns, enforce validation guards, and aggregate downstream provider APIs seamlessly:

1. **Provider Pattern (Abstraction)**
   - Concrete hotel search and booking integrations implement the `IHotelProvider` interface.
   - The application relies on two deterministic stubs:
     - `PremierStaysProvider` (modeling premium listings, pascal-case responses, and high star-ratings)
     - `BudgetNestsProvider` (modeling budget listings, snake-case responses, and basic structures)
   - Stubs are deterministic and contain a curated database of 5 cities: 2 domestic (Hyderabad, Bangalore) and 3 international (London, Paris, Dubai).

2. **Clean Architecture / Separated Layers**
   - **Endpoints (Presentation)**: Minimal APIs handling client requests, binding query parameters, executing validators, and returning HTTP responses.
   - **Services (Business Logic)**: Coordinates multi-provider searches in parallel, normalizes response types, filters unavailable options, applies pricing arithmetic, and executes booking validation rules.
   - **Validators (Application / Validation)**: Fluent Validation guards that enforce input requirements and travel document regulations server-side.
   - **Persistence (Data Store)**: EF Core DbContext for managing local transaction records (booking confirmations).

3. **CORS Support**
   - Configured dynamically in `Program.cs` to enable secure browser communication with the Angular client (running locally on `http://localhost:4200`).

---

## 📡 API Endpoints

All endpoints are prefixed with `/api/v1`.

### 1. GET `/api/v1/hotels/search`
Queries all active hotel providers in parallel, normalizes room types, calculates pricing, and returns a unified listing.
- **Parameters**:
  - `destination` (String, Required) - Must be a valid destination.
  - `checkIn` (DateOnly, Required) - Must be today or in the future.
  - `checkOut` (DateOnly, Required) - Must be after the check-in date.
  - `roomType` (String, Optional) - Filters results to a specific type (`Standard`, `Deluxe`, `Suite`). If omitted, returns all.

### 2. POST `/api/v1/hotels/book`
Enforces document validation guards (Passport vs National ID depending on destination) and completes a reservation with the selected provider, storing it in the database.
- **Payload**:
  - Contains passenger details, chosen room, and document configuration.
  - Returns `422 Unprocessable Entity` if travel documents do not match destination requirements.

### 3. GET `/api/v1/hotels/booking/{reference}`
Retrieves confirmation details and current reservation status by reference number.

---

## 🛂 Document Validation Rules

| Destination Type | Cities Included | Validation Rule |
| :--- | :--- | :--- |
| **Domestic** | Hyderabad, Bangalore | **National ID** or Passport is accepted |
| **International** | London, Paris, Dubai | **Passport** is strictly required |

---

## 🚀 Running the Application

### Prerequisites
- .NET 8.0 SDK or higher

### Commands
From the directory `d:\Praveen_Github_Projects\hotelstay-booking-platform`:

```bash
# Restore dependencies and build
dotnet build

# Run unit tests
dotnet test

# Start the Minimal API server
dotnet run --project HotelStay.Api
```

Once running, you can explore the endpoints via the Swagger UI at:
👉 **`http://localhost:5000/swagger`**
