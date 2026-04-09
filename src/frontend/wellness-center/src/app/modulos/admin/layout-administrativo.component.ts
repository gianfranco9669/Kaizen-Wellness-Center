import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ServicioAutenticacion } from '../../nucleo/servicios/servicio-autenticacion';

@Component({
  selector: 'app-layout-administrativo',
  standalone: true,
  imports: [RouterLink, RouterOutlet],
  template: `
  <div class="layout">
    <aside class="menu">
      <h2>Kaizen</h2>
      <a routerLink="/dashboard">Dashboard</a>
      <a routerLink="/clientes">Clientes</a>
      <a routerLink="/punto-venta">Punto de venta</a>
      <a routerLink="/cocina-tiempo-real">Cocina</a>
      <a routerLink="/recepcion-gimnasio">Recepcion gimnasio</a>
      <a routerLink="/portal-socio">Portal socio</a>
      <button (click)="salir()">Cerrar sesion</button>
    </aside>
    <section class="contenido"><router-outlet /></section>
  </div>`
})
export class LayoutAdministrativoComponent {
  constructor(private readonly auth: ServicioAutenticacion) {}
  salir(): void { this.auth.cerrarSesion(); }
}
