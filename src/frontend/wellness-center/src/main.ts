import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { rutasAplicacion } from './app/app.routes';
import { interceptorToken } from './app/nucleo/interceptores/token.interceptor';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(rutasAplicacion),
    provideHttpClient(withInterceptors([interceptorToken]))
  ]
}).catch(err => console.error(err));
