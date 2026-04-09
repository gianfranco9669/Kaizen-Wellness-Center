import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ServicioAutenticacion } from '../../nucleo/servicios/servicio-autenticacion';

@Component({
  selector: 'app-layout-administrativo',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
  <div class="layout-principal">
    <aside class="sidebar">
      <div class="logo-kaizen">
        <h2>Kaizen Wellness</h2>
        <span>Plataforma operativa premium</span>
      </div>

      <div>
        <div class="menu-grupo-titulo">Operacion</div>
        <a routerLink="/dashboard" routerLinkActive="activo" class="menu-link">
          <span class="icono">◼</span><span class="etiqueta">Dashboard</span>
        </a>
        <a routerLink="/punto-venta" routerLinkActive="activo" class="menu-link">
          <span class="icono">◉</span><span class="etiqueta">Punto de venta</span><span class="badge">POS</span>
        </a>
        <a routerLink="/cocina-tiempo-real" routerLinkActive="activo" class="menu-link">
          <span class="icono">☰</span><span class="etiqueta">Cocina</span>
        </a>
      </div>

      <div>
        <div class="menu-grupo-titulo">Clientes y socios</div>
        <a routerLink="/clientes" routerLinkActive="activo" class="menu-link">
          <span class="icono">◎</span><span class="etiqueta">Clientes</span>
        </a>
        <a routerLink="/recepcion-gimnasio" routerLinkActive="activo" class="menu-link">
          <span class="icono">◌</span><span class="etiqueta">Recepcion gimnasio</span>
        </a>
        <a routerLink="/portal-socio" routerLinkActive="activo" class="menu-link">
          <span class="icono">◍</span><span class="etiqueta">Portal socio</span><span class="badge">PWA</span>
        </a>
      </div>

      <button class="boton-salir" (click)="salir()">Cerrar sesion</button>
    </aside>

    <section class="area-contenido">
      <header class="topbar">
        <div>
          <div class="estado">Sistema operativo en linea</div>
          <div class="meta">Sede central · turno mediodia · sincronizacion activa</div>
        </div>
        <div class="meta">Kaizen Wellness Center</div>
      </header>

      <router-outlet />
    </section>
  </div>`
})
export class LayoutAdministrativoComponent {
  constructor(private readonly auth: ServicioAutenticacion) {}
  salir(): void { this.auth.cerrarSesion(); }
}
