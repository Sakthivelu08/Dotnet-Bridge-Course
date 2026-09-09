# Week 5 MCQ Practice — React & Full-Stack Integration

Prepare for your closed-book proctored exams with these core questions on React, routing, CORS, and full-stack JWT validation.

---

### Q1: What is the main purpose of an Axios Interceptor in full-stack JWT authentication?
- [ ] A) To encrypt the payload of all outgoing requests using AES-GCM.
- [x] B) To intercept outgoing requests and dynamically attach the `Authorization: Bearer <token>` header if a token is present in storage.
- [ ] C) To prevent CORS errors by routing requests through a local proxy server.
- [ ] D) To automatically redirect the browser to the login page when a token expires.

*Explanation:* An interceptor runs before every outgoing request, making it the perfect place to fetch the JWT token from `localStorage` and inject it into the request headers.

---

### Q2: Why does a Vite React application fail with a CORS (Cross-Origin Resource Sharing) error when trying to fetch data from a .NET API running on localhost?
- [ ] A) Vite does not support SSL (HTTPS) connections.
- [ ] B) The React application does not send the JWT token in the preflight request.
- [x] C) The browser blocks cross-origin requests unless the API server explicitly sends headers allowing the React origin.
- [ ] D) CORS is a C# compiler restriction that requires installing a Nuget package on the client.

*Explanation:* CORS is a browser security mechanism. It blocks client scripts from reading responses from a different origin (domain, port, or protocol) unless the server specifically responds with `Access-Control-Allow-Origin`.

---

### Q3: What is the risk of placing `app.UseCors(...)` after `app.UseAuthentication()` in the .NET request pipeline?
- [ ] A) The database context will fail to initialize.
- [ ] B) Password hashes will be transmitted in plain text.
- [x] C) Preflight `OPTIONS` requests sent by the browser will be blocked by authentication before CORS can approve the origin, causing the subsequent request to fail.
- [ ] D) JWT signatures cannot be validated.

*Explanation:* Browsers send an HTTP `OPTIONS` request (preflight check) before sending headers like `Authorization`. Since the preflight check doesn't contain a JWT token, `UseAuthentication` will reject it if placed before CORS, causing a CORS error in the browser.

---

### Q4: In a Vite React application, why must environment variables be prefixed with `VITE_`?
- [ ] A) Because Vite cannot compile standard JavaScript variables.
- [x] B) To prevent accidentally exposing private operating system environment variables to the public client bundle.
- [ ] C) Because the Node.js compiler forces the prefix at runtime.
- [ ] D) To bypass Webpack compilation requirements.

*Explanation:* Vite requires the `VITE_` prefix as a security boundary. Any variable without the prefix is ignored and will not be compiled into your client-side JavaScript.

---

### Q5: What is the difference between `process.env` and `import.meta.env`?
- [ ] A) `process.env` is faster than `import.meta.env`.
- [x] B) `process.env` is used in Node.js/Webpack server environments, while `import.meta.env` is used in modern ESM build tools like Vite.
- [ ] C) `import.meta.env` only works with TypeScript projects.
- [ ] D) `process.env` is read-only, whereas `import.meta.env` can be written to.

*Explanation:* Vite leverages ES Modules (ESM) where configuration metadata is accessed via `import.meta`, while older bundlers like Webpack run inside a Node.js process using `process.env`.
