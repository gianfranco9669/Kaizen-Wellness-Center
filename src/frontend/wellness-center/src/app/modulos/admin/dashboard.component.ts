import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
  <h1>Dashboard ejecutivo</h1>
  <div class="grid-kpi">
    <article class="tarjeta"><h3>Ventas dia</h3><strong>$ 1.250.000</strong></article>
    <article class="tarjeta"><h3>Ticket promedio</h3><strong>$ 14.800</strong></article>
    <article class="tarjeta"><h3>Socios activos</h3><strong>1.248</strong></article>
    <article class="tarjeta"><h3>Merma mensual</h3><strong>2.4 %</strong></article>
  </div>`
})
export class DashboardComponent {}
