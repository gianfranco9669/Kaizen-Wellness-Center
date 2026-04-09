import { Component } from '@angular/core';

@Component({
  selector: 'app-portal-socio',
  standalone: true,
  template: `
  <h1>Portal socio</h1>
  <div class="grid-kpi">
    <article class="tarjeta"><h3>Plan</h3><p>Premium Integral</p></article>
    <article class="tarjeta"><h3>Vencimiento</h3><p>30/05/2026</p></article>
    <article class="tarjeta"><h3>QR acceso</h3><p>Codigo dinamico habilitado</p></article>
  </div>`
})
export class PortalSocioComponent {}
