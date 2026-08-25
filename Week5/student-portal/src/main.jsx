import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import { theme } from './theme'

// Dynamically inject color tokens from theme.js as CSS variables into root document
Object.entries(theme).forEach(([key, value]) => {
  document.documentElement.style.setProperty(`--theme-${key}`, value);
});

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
