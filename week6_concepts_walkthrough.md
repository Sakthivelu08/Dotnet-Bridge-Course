# Week 6 Walkthrough — Full-Stack Integration (Part 2) & Testing

This document details the completion of the full-stack portal integration, test consolidation, error handling interceptors, and Selenium E2E test suites.

## 1. Full-Stack Integration & Error Handling

### Response Interceptors (`services/api.js`)
* **401 Unauthorized**: Automatically clears JWT tokens from `localStorage` and redirects the browser to `/login`.
* **403 Forbidden**: Displays a user-friendly access denied notification when a student attempts restricted operations.
* **400 Bad Request**: Gracefully handles form validation errors.
* **500 Internal Server Error**: Displays a server error fallback message without crashing the UI.

### Server-Side 403 Enforcement (`StudentsController.cs`)
* Enforced `[Authorize(Roles = "Teacher")]` on `POST /api/students` and `DELETE /api/students/{id}`. Direct HTTP requests from a Student user bypass client buttons but receive a `403 Forbidden` rejection from the server.

---

## 2. Test Architecture (3-Tier Testing Pyramid)

1. **Backend Unit Tests (xUnit + Moq)**:
   * Consolidate repository and service logic tests with mocked dependencies to ensure no direct DB calls during unit test runs. Hits $\ge 80\%$ line coverage.
2. **Frontend Unit Tests (Vitest + RTL)**:
   * 12 test cases covering component states, loading spinners, empty states, search filtering, controlled input validations, and role visibility. Hits $82.09\%$ line coverage.
3. **Selenium E2E Tests (C# WebDriver)**:
   * Located in `Week6/tests/BridgeCourse.Week6.E2E/SeleniumE2ETests.cs`.
   * **Smoke Test**: Launches Chrome and asserts `/login` page loads.
   * **Flow 1**: Register + Login with JWT session validation.
   * **Flow 2**: Teacher Full CRUD workflow.
   * **Flow 3**: Student Read-Only view enforcement.
   * **Flow 4**: Logout workflow.
   * **Negative Test**: Asserts Student direct write POST attempt is rejected by the server with a `403 Forbidden` response.
