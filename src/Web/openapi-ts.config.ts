import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'http://localhost:7010/openapi/v1.json',
  output: { path: 'src/generated' },
  plugins: [
    { name: '@hey-api/client-fetch', runtimeConfigPath: './src/lib/heyApiConfig.ts' },
    { name: '@tanstack/react-query' },
  ],
});
