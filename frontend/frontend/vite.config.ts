import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'


// In dev we proxy /api to the local .NET backend at :8080
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': 'http://localhost:8080'
    }
  },
  build: {
    sourcemap: true
  }
})
