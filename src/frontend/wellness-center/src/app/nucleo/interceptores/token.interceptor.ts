import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ServicioAutenticacion } from '../servicios/servicio-autenticacion';

export const interceptorToken: HttpInterceptorFn = (request, next) => {
  const auth = inject(ServicioAutenticacion);
  const token = auth.obtenerToken();

  if (token) {
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(request);
};
