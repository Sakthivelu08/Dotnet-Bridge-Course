# Week 5 Walkthrough — React Full-Stack Integration

This document outlines the walkthrough and verification details of the **Student Directory Portal** full-stack integration developed during Week 5.

## 1. Features Shipped

### .NET Backend Role Security & Privacy
* **Dynamic JWT claims**: Authenticates user logins and issues a signed token containing role (`Student` or `Teacher`) and registration email claims.
* **Student-Level Privacy Filter**: Updated the `/api/students` `GET` route to intercept student logins and filter database results to only return their own profile (students cannot view other students). Teachers see all profiles.
* **Teacher Write Restrictions**: Enforced `[Authorize(Roles = "Teacher")]` on create and delete endpoints.

### React SPA Portal
* **Routing & Authentication**: Implemented `react-router-dom` routing with a `ProtectedRoute` redirect wrapper ensuring unauthenticated requests are redirected back to `/login`.
* **Dynamic Navigation Link Visibility**: Navigation links to Login/Register are automatically hidden once the user logs in, showing only Directory and Logout options.
* **Live Search Filtering**: Performs high-performance client-side string filtering on student rows and updates a match counter.
* **Conditional UI rendering**: Shows the Create Form and Delete buttons only when the active role is `"Teacher"`.

---

## 2. Testing Focus & Coverage
The project includes a Vitest + React Testing Library suite located in the `src/` directory.

### Running the Test Suite:
To run the automated tests, navigate to the `student-portal` folder and run:
```bash
npm run test
```

To view the coverage summary and ensure it exceeds the **80% coverage threshold**:
```bash
npm run test:coverage
```

### Coverage Assertions:
1. **`StudentList.test.jsx`**: Validates row rendering from props, empty state banner rendering, and loading message.
2. **`Register.test.jsx`**: Confirms form submission button is disabled until all inputs pass validation.
3. **`Students.test.jsx`**: Asserts search queries filter student listings live, and validates that write controls are hidden for students and shown for teachers.
