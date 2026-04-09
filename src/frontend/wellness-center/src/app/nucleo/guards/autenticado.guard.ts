import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ServicioAutenticacion } from '../servicios/servicio-autenticacion';

export const guardAutenticado: CanActivateFn = () => {
  const auth = inject(ServicioAutenticacion);
  const router = inject(Router);

  if (auth.autenticado()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
