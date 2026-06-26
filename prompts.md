# Developer Prompt History - HotelStay Booking Platform

This file records the series of structured prompts and instructions used during the implementation and refinement of the HotelStay Booking Platform.

---

## Phase 1: Architectural Foundation & Backend Project Setup

### Prompt 1.1: Web API Project Creation and Structural Layout
> **Prompt:**
> Create a new C# Solution `HotelStay.sln` containing two projects:
> 1. `HotelStay.Api`: An ASP.NET Core 10 Minimal API project.
> 2. `HotelStay.Tests`: A xUnit unit test project.
>
> Organize `HotelStay.Api` using Clean Architecture principles:
> - `DomainEntities`: Domain models like `Hotel`, `Room`, `Booking`.
> - `Enums`: Enums like `RoomType` (Standard = 1, Deluxe = 2, Suite = 3) and `DocumentType` (Passport = 1, NationalId = 2).
> - `Persistence`: EF Core DB context (`HotelDbContext`) using PostgreSQL, migrations, and a seed data helper (`SeedData`) to seed 5 default cities (Hyderabad, Bangalore, London, Paris, Dubai).
> - `ApplicationInterfaces`: Core abstractions like `IHotelProvider`, `IBookingProvider`, `IHotelSearchService`, `IBookingService`, and `IDocumentValidationService`.
> - `InfrastructureProviders`: Concrete implementations of `IHotelProvider` (to start, stub stubs for `PremierStaysProvider` and `BudgetNestsProvider`).
> - `Services`: Core services implementation.
> - `Validators`: Request validation using FluentValidation.
> - `Endpoints`: Grouped Minimal API endpoints.
> - `Models`: Request and response DTO records.

### Prompt 1.2: Database Seeding & Persistence Integration
> **Prompt:**
> Write `HotelDbContext.cs` and `SeedData.cs`. Seed the database with the following structure:
> - Hotels in 5 cities: Hyderabad, Bangalore (Domestic) and London, Paris, Dubai (International).
> - Map rooms to these hotels with different pricing structures (in local or standardized currencies).
> - Enforce primary and foreign key constraints on the Bookings table to store traveler details (Name, Document Type, Document Number, total price, cancellation policy).

---

## Phase 2: Hotel Provider Integration (Stubbing & Parallel Aggregation)

### Prompt 2.1: Defining Provider Integration Contracts
> **Prompt:**
> Write the `IHotelProvider` interface with:
> - `string ProviderName { get; }`
> - `Task<List<HotelResult>> SearchAsync(SearchRequest request, CancellationToken cancellationToken)`
> - `Task BookAsync(BookingRequest request, CancellationToken cancellationToken)`
>
> Create two mock providers:
> 1. `PremierStaysProvider`:
>    - Uses PascalCase fields.
>    - Returns full hotel details (amenities, star rating, average rating).
>    - Returns cancellation policies (e.g. Free cancellation, Non-refundable).
>    - Always returns available rooms.
> 2. `BudgetNestsProvider`:
>    - Uses snake_case fields internally (emulate external API payload mapping).
>    - Minimal details (no ratings or amenities).
>    - Can return unavailable rooms (`available: false`). These must be filtered out in the aggregation service.
> Ensure both return rates as per-night amounts.

### Prompt 2.2: Implement Parallel Search Aggregator
> **Prompt:**
> Write `HotelSearchService.cs` implementing `IHotelSearchService`.
> - It should execute searches across all registered `IHotelProvider` instances in parallel using `Task.WhenAll`.
> - If one provider fails (throws an exception), the aggregator should log the failure and still return results from other healthy providers instead of failing the entire request.
> - Filter out any rooms where `IsAvailable == false`.
> - Map room type representations into the unified enum `RoomType`.
> - Calculate total stay price using: `Total Price = Nightly Rate * Number of Nights`.

---

## Phase 3: Validation, Booking Workflows & API Endpoints

### Prompt 3.1: Server-Side FluentValidation & Travel Rules
> **Prompt:**
> Write FluentValidation validators for:
> 1. `SearchHotelsQueryValidator`:
>    - `destination`, `checkIn`, `checkOut` are required.
>    - `checkOut` must be after `checkIn`.
> 2. `BookingRequestValidator`:
>    - Passenger details (names, email) must be validated.
>    - `DocumentNumber` must be alphanumeric. Passport: 6-9 characters. National ID: 6-20 characters.
>    - Implement travel document rules based on destination:
>      - Domestic (Hyderabad, Bangalore): Accept Passport or National ID.
>      - International (London, Paris, Dubai): Passport strictly required.
>      - If validation fails, return `HTTP 422 Unprocessable Entity` with a clear message: `"Passport required for international destinations"`.

### Prompt 3.2: API Endpoints Routing
> **Prompt:**
> Implement Minimal API endpoints mapped under `/api/v1`:
> - `GET /api/v1/hotels/search`: Calls search service and returns unified results.
> - `POST /api/v1/hotels/book`: Validates request and creates booking in database. Returns booking confirmation reference (e.g., `HS-YYYYMMDD-XXXX`).
> - `GET /api/v1/hotels/booking/{reference}`: Retrieves booking status and details.
> Include Global Error Handling and CORS policy allowing `http://localhost:4200` to access endpoints.

---

## Phase 4: Frontend Development (Angular 20 & UI Framework Setup)

### Prompt 4.1: Initializing Angular Application with Standalone Components
> **Prompt:**
> Create an Angular application `hotelstay-ui` using Angular version 20:
> - Use standalone components for routing and layout.
> - Configure routes:
>   - `/`: Search page
>   - `/results`: Search results list
>   - `/book/:hotelId`: Booking form
>   - `/confirmation/:reference`: Confirmation screen
> - Create core API services: `HotelSearchService` and `BookingService` injecting `HttpClient` to communicate with ASP.NET Core backend.
> - Set up environment variables for the API base URL.

---

## Phase 5: UX/UI Enhancement with Angular Material Components

### Prompt 5.1: Applying Material Styling & Professional Layout
> **Prompt:**
> Replace raw HTML elements in the Angular UI with Angular Material components:
> - Install `@angular/material` and import `MatFormFieldModule`, `MatInputModule`, `MatDatepickerModule`, `MatNativeDateModule`, `MatButtonModule`, `MatChipsModule`, `MatCardModule`, `MatIconModule`, and `MatSelectModule`.
> - Configure `provideAnimationsAsync()` in `app.config.ts`.
> - Build a responsive grid for hotel result cards.
> - Add a per-provider badge with distinctive icons (`star` for PremierStays, `local_offer` for BudgetNests).
> - Style cancellation policies using `<mat-chip-set>` (green chip for refundable, red chip for non-refundable).
> - Implement Client-Side Validation validation matching server-side rules. Use `<mat-error>` to show required messages and prevent vertical layout shifts.
> - Add a "Back to Results" button on the booking page.

---

## Phase 6: Live Tweak - Integrating BoutiqueCollection Provider

### Prompt 6.1: BoutiqueCollection Implementation (Extensibility Test)
> **Prompt:**
> Implement the BoutiqueCollection provider following the Extensibility design of the application.
> Requirements:
> - Create `BoutiqueCollectionProvider.cs` implementing `IHotelProvider`.
> - It should only support `Deluxe` and `Suite` room types (Standard rooms must return unavailable).
> - Apply a Boutique Fee of £15 per night (added to the nightly rate of the listing).
> - Cancellation policy: "FreeCancellation up to 72h before check-in".
> - Register the new provider in the DI container in `DependencyInjection.cs` using:
>   `services.AddScoped<IHotelProvider, BoutiqueCollectionProvider>();`
> Verify that the Search Aggregator picks up BoutiqueCollection results automatically without editing the aggregator or booking services.

---

## Phase 7: Comprehensive Testing

### Prompt 7.1: xUnit Backend Tests
> **Prompt:**
> Write unit tests in `HotelStay.Tests` to cover:
> - Provider results mapping and normalization.
> - Parallel execution in search aggregator.
> - Aggregator resilience (one provider throwing exception does not crash search).
> - Room availability filtering (available rooms kept, unavailable rooms filtered out).
> - Travel document validation logic (correct error codes and HTTP responses).
> - Booking flow persistence and retrieval.

### Prompt 7.2: Frontend Unit Testing
> **Prompt:**
> Write Vitest tests in the Angular project to test:
> - Search form submission and navigation.
> - Form control error states showing in `<mat-error>`.
> - Hotel card component rendering correct provider badges and cancellation policy chips.
> - Routing parameter bindings on the confirmation and results pages.

---

## Phase 8: Solution Architecture Audit & candidate Refinements

### Prompt 8.1: Architecture Clean-up and Bug Resolution
> **Prompt:**
> Refactor the project to fix all logic bugs, SOLID violations, and architecture gaps identified in the Senior Architect audit:
> 1. Make the aggregator filtering generic (`result.IsAvailable` filter instead of matching `"BudgetNests"` provider name) to align with OCP.
> 2. Add convention-based provider code initials mapping inside `BookingService` to dynamically generate references like `HSB-BC-...` for BoutiqueCollection.
> 3. Expand the `Booking` database model to store dates of stay, hotel name, hotel ID, guest count, and currency, fixing state loss on confirmation page reloads.
> 4. Implement the `GET /hotels/{hotelId}` backend route to enable seamless booking page reloads.
> 5. Configure global exception handling middleware in the API pipeline to map validation and operation errors to structured 422/400 responses.
> 6. Secure document validation boundaries by verifying destination cities against allowed domestic/international city lists.
> 7. Clean up dead code by removing unused database tables (`Hotels`, `Rooms`, `Guests`) and their configuration/seeding logic.
> 8. Replace the search destination text input with a strict dropdown menu on the frontend and add custom CSS classes for the BoutiqueCollection themed UI cards.
