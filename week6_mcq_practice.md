# Week 6 MCQ Practice — E2E Testing & Full-Stack Regression

Prepare for your closed-book proctored exams with these core questions on Selenium WebDriver, testing pyramids, and error handling.

---

### Q1: What is the primary difference between Unit Tests (xUnit/Vitest) and End-to-End Tests (Selenium)?
- [ ] A) Unit tests require a running browser, while E2E tests run in memory.
- [ ] B) Unit tests require real database connections, while E2E tests mock the database.
- [x] C) Unit tests test isolated code units with mocked dependencies, whereas E2E tests validate the complete browser workflow against the real running application stack.
- [ ] D) E2E tests count toward the 80% line coverage requirement, while unit tests do not.

*Explanation:* Unit tests isolate small methods and mock external dependencies (like DBs or network calls). E2E tests run in a real browser to prove the entire user journey works end-to-end.

---

### Q2: How does an Axios response interceptor handle an HTTP 401 Unauthorized response?
- [ ] A) It retries the request 5 times automatically.
- [x] B) It intercepts the 401 status, clears expired JWT tokens from `localStorage`, and redirects the user to the `/login` page.
- [ ] C) It converts the response into an HTTP 200 OK.
- [ ] D) It sends a POST request to create a new user account.

*Explanation:* An HTTP 401 status indicates an unauthenticated or expired token. The interceptor intercepts this status globally to clean up local credentials and force re-authentication.

---

### Q3: Why is server-side role gatekeeping `[Authorize(Roles = "Teacher")]` mandatory even if client-side React UI hides the "Delete" button from Students?
- [ ] A) Because React components cannot execute Javascript without C# server code.
- [ ] B) Because CSS styles can be bypassed by zooming out in the browser.
- [x] C) Because a client-side button check is only a visual convenience; a malicious student can bypass the UI and make direct HTTP API requests using tools like Postman or browser console fetches.
- [ ] D) Because EF Core cannot save entities unless `[Authorize]` is present.

*Explanation:* Client-side checks are for user experience. True security must be enforced on the server to prevent direct API calls from unauthorized users.

---

### Q4: In Selenium WebDriver, why should explicit waits (`WebDriverWait` / `ExpectedConditions`) be used instead of `Thread.Sleep()`?
- [ ] A) `Thread.Sleep()` causes memory leaks in C#.
- [x] B) `WebDriverWait` waits dynamically until the condition is met (resuming immediately), whereas `Thread.Sleep()` pauses execution unconditionally, slowing down test suites.
- [ ] C) `Thread.Sleep()` is deprecated in .NET 8.
- [ ] D) `WebDriverWait` automatically installs Chrome browser updates.

*Explanation:* Explicit waits poll the DOM dynamically and resume as soon as the element is visible, optimizing test execution speed and eliminating flaky timing errors.

---

### Q5: What is the purpose of running tests in a "Clean Checkout" before a final submission demo?
- [ ] A) To delete all git commit history.
- [ ] B) To reset the operating system environment variables.
- [x] C) To verify that the full-stack portal builds, compiles, and passes unit + E2E test suites from scratch without depending on local cached state or uncommitted files.
- [ ] D) To automatically upload code coverage reports to GitHub.

*Explanation:* A clean checkout verification guarantees that the codebase is completely self-contained and reproducible by evaluators or CI/CD build agents.
