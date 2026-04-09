import { Component } from '@angular/core';

@Component({
  selector: 'app-pantalla-cocina',
  standalone: true,
  template: `
  <h1>Cocina tiempo real</h1>
  <div class="grid-kpi">
    <article class="tarjeta"><h3>Pendientes</h3><p>12 pedidos</p></article>
    <article class="tarjeta"><h3>En preparacion</h3><p>8 pedidos</p></article>
    <article class="tarjeta"><h3>Listos</h3><p>4 pedidos</p></article>
  </div>`
})
export class PantallaCocinaComponent {}
