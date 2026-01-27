import { defineConfig } from 'vite';
import path from 'path';

export default defineConfig({
  envDir: path.resolve(__dirname, '../../'),
  build: {
    emptyOutDir: false,
    minify: false,
    outDir: './wwwroot/',
    cssCodeSplit: true,
    rollupOptions: {
      input: {
        jobs: './Scripts/pages/jobs.ts', 
        hire: './Scripts/pages/hire.ts', 
        login: './Scripts/pages/login.ts', 
        empty: './Scripts/pages/empty.ts', 
        questions: './Scripts/pages/questions.ts', 
        applications: './Scripts/pages/applications.ts', 
        "magic-link": './Scripts/pages/magic-link.ts', 
        "dark-theme": './Scripts/shared/dark-theme.ts', 
      },
      output: {
        entryFileNames: 'js/[name].js',  // Nombre del archivo de salida de JavaScript
        chunkFileNames: 'js/[name]-[hash].js',  // Nombre del archivo de salida de JavaScript
        // assetFileNames: 'css/styles.css'
        // assetFileNames: 'css/[name].[ext]'
        assetFileNames: (assetInfo) => {
          
          if (assetInfo.names.includes("dark-theme.css"))
          {
            return "css/[name].[ext]";
          }
          else
          {
            return "css/styles.css";
          }
        },
      },
    },
  },
  css: {
    postcss: {
      plugins: [
        // Si quieres usar algún plugin de PostCSS puedes configurarlo aquí
      ],
    },
  },

  /* more info: 
    - https://salteadorneo.dev/blog/alias-rutas-vite/ 
    - https://vueschool.io/articles/vuejs-tutorials/import-aliases-in-vite/
    - https://stackoverflow.com/questions/75601350/tailwind-and-vite-warnings-didnt-resolve-at-build-time-it-will-remain-unchan
  */
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './Scripts'),
      '@styles': path.resolve(__dirname, './Styles'),
    },
  },

});