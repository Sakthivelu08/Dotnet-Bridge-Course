import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/setupTests.js',
    coverage: {
      provider: 'v8',
      include: [
        'src/components/**/*', 
        'src/pages/**/*', 
        'src/services/api.js',
        'src/main.jsx',
        'src/theme.js'
      ],
      exclude: ['src/setupTests.js'],
      thresholds: {
        lines: 80,
        branches: 70,
        functions: 80,
        statements: 80,
      }
    }
  }
})
