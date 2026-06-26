# spec.md

# HotelStay Booking Platform - Technical Specification

## Project Overview

HotelStay is a hotel search and booking platform developed for the SkyRoute Travel Platform.

The application aggregates hotel availability and pricing from multiple providers and presents a unified booking experience to travelers.

The platform allows users to:

* Search hotels by destination and travel dates
* Compare pricing from multiple providers
* View cancellation policies
* Complete bookings
* Retrieve booking status using a booking reference

---

# Business Requirements

## Search Hotels

Travellers can search hotels using:

* Destination
* Check-In Date
* Check-Out Date
* Room Type (Optional)

Supported Room Types:

* Standard
* Deluxe
* Suite

The system queries multiple hotel providers and returns normalized results.

---

## Hotel Providers

### PremierStays

Characteristics:

* Higher rates
* Full property details
* Amenities included
* Star rating included
* Cancellation policy included
* Always returns availability

Supported Cancellation Policies:

* FreeCancellation (48 hours before check-in)
* NonRefundable

Response format:

* PascalCase JSON

---

### BudgetNests

Characteristics:

* Lower rates
* Minimal hotel details
* No amenities
* No star rating
* May return unavailable rooms

Supported Cancellation Policies:

* Flexible (24 hours before check-in)
* NonRefundable

Response format:

* snake_case JSON

Unavailable rooms are filtered before returning results to the client.

---

### BoutiqueCollection

Characteristics:

* Supports Deluxe and Suite only
* Standard rooms are unavailable
* Base rates are subject to an additional boutique fee of £15 per night (applied regardless of room type)
* Free cancellation up to 72 hours before check-in
* Always returns availability as a boolean per room type

Supported Cancellation Policies:

* FreeCancellation (72 hours before check-in)

Response format:

* PascalCase JSON (extends IHotelProvider interface)

---

# Search Aggregation

The backend aggregates responses from all providers.

Responsibilities:

* Execute provider searches in parallel
* Normalize provider-specific responses
* Filter unavailable rooms
* Calculate total stay price
* Return unified hotel search results

---

# Pricing Rules

Providers return:

* Per-night rate

Frontend displays:

* Per-night price
* Total stay price

Formula:

Total Price = Nightly Rate × Number Of Nights

---

# Booking Workflow

Users can select a hotel result and proceed with booking.

Booking flow:

1. Validate traveler information
2. Validate destination document requirements
3. Submit booking to selected provider
4. Generate booking reference
5. Store booking information
6. Return confirmation

---

# Destination Validation Rules

## Domestic Destinations

Supported Domestic Cities:

* Hyderabad
* Bangalore

Accepted Documents:

* Passport
* National ID

---

## International Destinations

Supported International Cities:

* London
* Paris
* Dubai

Accepted Documents:

* Passport Only

National ID is not accepted.

Validation occurs on:

* Angular Frontend
* ASP.NET Core Backend

Invalid requests return:

HTTP 422 Unprocessable Entity

Example:

```json
{
  "message": "Passport required for international destinations"
}
```

---

# API Specification

## Search Hotels

Endpoint:

```http
GET /hotels/search
```

Query Parameters:

| Parameter   | Required | Description      |
| ----------- | -------- | ---------------- |
| destination | Yes      | Destination city |
| checkIn     | Yes      | Check-in date    |
| checkOut    | Yes      | Check-out date   |
| roomType    | No       | Room type filter |

Response:

```json
[
  {
    "provider": "PremierStays",
    "roomType": "Deluxe",
    "nightlyRate": 180,
    "totalPrice": 540,
    "cancellationPolicy": "FreeCancellation"
  }
]
```

---

## Create Booking

Endpoint:

```http
POST /hotels/book
```

Request:

```json
{
  "provider": "PremierStays",
  "destination": "London",
  "passengerName": "John Smith",
  "documentType": "Passport",
  "documentNumber": "P1234567"
}
```

Response:

```json
{
  "reference": "HTL-20260618-001",
  "provider": "PremierStays",
  "status": "Confirmed"
}
```

---

## Get Booking Status

Endpoint:

```http
GET /hotels/booking/{reference}
```

Response:

```json
{
  "reference": "HTL-20260618-001",
  "status": "Confirmed"
}
```

---

# System Architecture

## Backend

Technology Stack:

* ASP.NET Core 10 Minimal API
* Entity Framework Core
* EF Core In-Memory Database
* FluentValidation
* Swagger

Architecture Pattern:

* Clean Architecture
* Provider Pattern
* Dependency Injection
* Repository Pattern

---

## Frontend

Technology Stack:

* Angular 20
* Standalone Components
* Reactive Forms
* RxJS
* Angular Signals
* Angular Material

Features:

* Search Page
* Results Page
* Booking Page
* Confirmation Page

---

# Database Design

## Bookings Table

| Column             | Description             |
| ------------------ | ----------------------- |
| Id                 | Primary Key             |
| Reference          | Booking Reference       |
| Provider           | Hotel Provider          |
| PassengerName      | Traveler Name           |
| Destination        | Travel Destination      |
| RoomType           | Selected Room           |
| DocumentType       | Passport or National ID |
| DocumentNumber     | Traveler Document       |
| CancellationPolicy | Booking Policy          |
| TotalPrice         | Final Price             |
| Status             | Booking Status          |
| CreatedAt          | Booking Timestamp       |
| CheckInDate        | Check-In Date           |
| CheckOutDate       | Check-Out Date          |
| HotelId            | Hotel Identifier        |
| HotelName          | Hotel Name              |
| Adults             | Number of Adults        |
| Rooms              | Number of Rooms         |
| Currency           | Booking Currency        |

---

# Extensibility Design

The platform is designed to support additional hotel providers without modifying existing business logic.

New providers must:

* Implement IHotelProvider
* Register through Dependency Injection

No modifications are required in:

* Aggregation Service
* Booking Service
* Existing Provider Implementations

---

# Live Tweak Scenario (Completed)

The BoutiqueCollection provider was successfully integrated as part of the extensibility design validation:

* **Status**: Completed
* **Implementation Details**:
  - Implemented `BoutiqueCollectionProvider.cs` under the `InfrastructureProviders` folder.
  - Registered the provider inside `DependencyInjection.cs` container.
  - Validated that the search aggregation and booking orchestration pick up the new provider dynamically without modification to the core orchestrator or business logic.
  - Added new unit test assertions to verify BoutiqueCollection calculations (the £15 fee, RoomType restrictions, and cancellation terms).

---

# Testing Strategy

Unit tests cover:

* Provider normalization
* Search aggregation
* Room filtering
* Document validation
* Booking creation
* Booking retrieval
* Provider failure handling

---

# Deployment

Backend:

* Railway

Frontend:

* Vercel

Database:

* EF Core In-Memory Database

---

# Definition of Done

The implementation is considered complete when:

* Search aggregates multiple providers
* Unavailable rooms are filtered
* Pricing is calculated correctly
* Document validation works client-side and server-side
* Booking flow completes successfully
* Booking status retrieval works
* Tests pass
* dotnet build succeeds
* ng build succeeds
* Deployment succeeds
* No secrets are committed to source control
