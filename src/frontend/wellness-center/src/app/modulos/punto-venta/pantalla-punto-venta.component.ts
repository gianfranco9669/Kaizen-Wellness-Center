import { Component } from '@angular/core';

@Component({
  selector: 'app-punto-venta',
  standalone: true,
  template: `
  <h1>Punto de venta gastronomico</h1>
  <div class="grid-kpi">
    <article class="tarjeta"><h3>Pedido rapido</h3><p>Mostrador, take away, delivery.</p></article>
    <article class="tarjeta"><h3>Cobro mixto</h3><p>Efectivo, tarjeta, billetera virtual.</p></article>
  </div>`
})
export class PantallaPuntoVentaComponent {}
