# reflection.md

# HotelStay Booking Platform - Technical Reflection

## Overview

This project was developed as a hotel search and booking platform that aggregates hotel availability and pricing from multiple providers into a unified experience for travelers.

The solution includes:

* ASP.NET Core 10 Minimal API backend
* Angular 20 frontend
* PostgreSQL persistence
* Provider-based architecture
* Automated testing
* Railway deployment for backend
* Vercel deployment for frontend

The primary objective was not only to satisfy the functional requirements but also to create a maintainable, extensible, and production-oriented architecture.

---

# Architectural Decisions

## 1. Provider Pattern

One of the key requirements was the ability to add new hotel providers without modifying existing aggregation or booking orchestration logic.

To achieve this, I introduced an abstraction:

```csharp
public interface IHotelProvider
{
    Task<List<HotelResult>> SearchAsync(SearchRequest request);

    Task<BookingResponse> BookAsync(BookingRequest request);

    string ProviderName { get; }
}
```

Each provider implements this interface independently.

Current implementations:

* PremierStaysProvider
* BudgetNestsProvider

This design follows:

* Open/Closed Principle
* Dependency Inversion Principle
* Strategy Pattern concepts

Benefits:

* Easy onboarding of new providers
* Improved maintainability
* Simplified testing
* Reduced coupling

---

## 2. Clean Architecture

The solution was structured into distinct layers:

### Domain

Contains:

* Entities
* Enums
* Core business models

### Application

Contains:

* Interfaces
* Business services
* Validation rules

### Infrastructure

Contains:

* Provider implementations
* Database access
* Persistence concerns

### API

Contains:

* Minimal API endpoints
* Dependency injection configuration
* Application startup

Benefits:

* Separation of concerns
* Easier testing
* Better maintainability
* Reduced coupling between layers

---

## 3. Parallel Provider Execution

Hotel searches are executed against all providers concurrently.

Instead of:

```csharp
await provider1.SearchAsync();
await provider2.SearchAsync();
```

The implementation uses:

```csharp
Task.WhenAll(...)
```

Benefits:

* Faster response times
* Better scalability
* Improved user experience

---

## 4. Unified Data Model

Provider responses differ significantly.

For example:

PremierStays:

* PascalCase
* Amenities
* StarRating

BudgetNests:

* snake_case
* No amenities
* No star rating

To simplify frontend consumption, all provider responses are normalized into a single domain model.

Benefits:

* Simplified frontend implementation
* Reduced client-side mapping
* Consistent API contract

---

# Validation Strategy

Validation was intentionally implemented at multiple layers.

## Client-Side Validation

Angular Reactive Forms validate:

* Required fields
* Date ranges
* Document requirements

Benefits:

* Immediate user feedback
* Reduced unnecessary API calls

---

## Server-Side Validation

Backend validation acts as the final authority.

Implemented using:

* FluentValidation
* Business validation services

Benefits:

* Security
* Data integrity
* Consistency

---

# Document Validation Design

Business rules:

Domestic destinations:

* Passport accepted
* National ID accepted

International destinations:

* Passport required

Validation is enforced on both frontend and backend.

This prevents invalid bookings regardless of client implementation.

---

# Database Design

PostgreSQL was selected because:

* Strong EF Core support
* Production-ready
* Railway integration
* Open-source

The Booking entity stores:

* Booking reference
* Passenger details
* Provider information
* Pricing details
* Booking status

Entity Framework Core Code First was used to simplify schema evolution.

---

# Frontend Design Decisions

Angular 20 was selected because:

* Modern framework
* Strong TypeScript support
* Standalone Components
* Signals support
* Excellent Reactive Forms implementation

The frontend was divided into feature modules:

* Search
* Results
* Booking
* Confirmation

Benefits:

* Maintainability
* Scalability
* Clear separation of responsibilities

---

# Deployment Decisions

## Backend

Platform:

Railway

Reasons:

* Simple deployment workflow
* PostgreSQL integration
* CI/CD support
* Cost-effective

---

## Frontend

Platform:

Vercel

Reasons:

* Optimized Angular deployments
* Global CDN
* Fast build pipeline
* Simple GitHub integration

---

# Testing Strategy

Unit tests were created for:

* Provider normalization
* Unavailable room filtering
* Document validation
* Booking creation
* Booking retrieval
* Provider failure handling

Testing focused on business rules and critical workflows.

---

# AI-Assisted Development

AI tools used:

* GitHub Copilot
* Antigravity
* Codex
* ChatGPT

AI was used for:

* Initial scaffolding
* Boilerplate generation
* Test generation
* Documentation drafting
* Refactoring suggestions

All generated code was reviewed, validated, and modified where necessary.

Architectural decisions, implementation choices, and final verification remained the responsibility of the developer.

---

# Challenges Encountered

## PostgreSQL Connectivity

During development, local PostgreSQL connectivity issues were encountered due to service configuration and environment setup.

Resolution:

* Verified PostgreSQL service availability
* Updated connection strings
* Added environment-based configuration

---

## Railway Deployment Detection

Railway initially failed to detect the startup project.

Resolution:

* Explicit deployment configuration
* Startup project verification
* Build and publish validation

---

# Future Improvements

If additional time were available, I would consider:

* Authentication and authorization
* Distributed caching
* OpenTelemetry monitoring
* CQRS pattern
* Provider retry policies
* Background booking processing
* Docker containerization
* Integration testing
* Rate limiting

---

# Conclusion

The final solution emphasizes maintainability, extensibility, and clean architecture while satisfying all functional requirements.

The provider-based design allows new hotel providers to be added with minimal effort, making the platform scalable for future growth.

The combination of ASP.NET Core 10, Angular 20, PostgreSQL, and modern engineering practices provides a solid foundation for a real-world hotel booking platform.
