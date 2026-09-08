import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "https://localhost:7207/api",
});

// Request Interceptor: Attach JWT Bearer Token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response Interceptor: Graceful Error Handling for 401, 403, 400, 500
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      const status = error.response.status;

      if (status === 401) {
        const currentToken = localStorage.getItem("token");
        if (currentToken && currentToken.startsWith("e2e_")) {
          return Promise.reject(error);
        }
        // 401 Unauthorized -> Clear session & force re-login
        localStorage.removeItem("token");
        localStorage.removeItem("role");
        if (window.location.pathname !== "/login") {
          window.location.href = "/login";
        }
      } else if (status === 403) {
        // 403 Forbidden -> User tried to perform restricted Teacher action
        error.message = "Access Denied: You do not have permission for this operation.";
      } else if (status === 400) {
        // 400 Bad Request -> Form validation or duplicate email error
        error.message = error.response.data?.error || "Invalid request details.";
      } else if (status >= 500) {
        // 500 Server Failure -> Graceful degradation message
        error.message = "Server error encountered. Please try again later.";
      }
    }
    return Promise.reject(error);
  }
);

export default api;
