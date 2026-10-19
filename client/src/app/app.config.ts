import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { mockApiInterceptor } from './core/mock/mock-api-interceptor';
import { loadingInterceptor } from './core/interceptors/loading-interceptor';
import { errorInterceptor } from './core/interceptors/error-interceptor';
import { MatIconRegistry } from '@angular/material/icon';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAppInitializer(() => {
      inject(MatIconRegistry).setDefaultFontSetClass('material-symbols-outlined');
    }),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([loadingInterceptor, errorInterceptor, mockApiInterceptor]))
  ]
};
