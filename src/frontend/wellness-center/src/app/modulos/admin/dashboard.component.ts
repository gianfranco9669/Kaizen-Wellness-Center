import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
  <section class="panel">
    <h1>Dashboard ejecutivo</h1>
    <p>Resumen operativo de gastronomia, gimnasio y finanzas del dia.</p>

    <div class="grid-kpi" style="margin-top:12px;">
      <article class="kpi"><div>Ventas del dia</div><div class="valor">$ 2.140.000</div></article>
      <article class="kpi"><div>Pedidos del dia</div><div class="valor">186</div></article>
      <article class="kpi"><div>Socios activos</div><div class="valor">1.324</div></article>
      <article class="kpi"><div>Vencen hoy</div><div class="valor">19</div></article>
      <article class="kpi"><div>Merma estimada</div><div class="valor">2.1%</div></article>
      <article class="kpi"><div>Caja neta</div><div class="valor">$ 890.000</div></article>
    </div>
  </section>

  <section class="grid-dos" style="margin-top:12px;">
    <article class="panel">
      <h3>Actividad reciente</h3>
      <ul class="lista-simple" style="margin-top:10px;">
        <li class="item-simple"><strong>Pedido #P-1082</strong> listo para entrega · canal delivery</li>
        <li class="item-simple"><strong>Socio #SOC-1002</strong> realizo check-in en recepcion</li>
        <li class="item-simple"><strong>Alerta stock</strong> salmon premium por debajo de minimo</li>
        <li class="item-simple"><strong>Cierre parcial caja</strong> turno mediodia confirmado</li>
      </ul>
    </article>

    <article class="panel">
      <h3>Alertas gerenciales</h3>
      <ul class="lista-simple" style="margin-top:10px;">
        <li class="item-simple"><span class="pastilla alerta">Atencion</span> 4 membresias vencen en 48h</li>
        <li class="item-simple"><span class="pastilla error">Riesgo</span> Merma arroz sushi +18% vs promedio</li>
        <li class="item-simple"><span class="pastilla ok">OK</span> Integracion pagos sincronizada</li>
      </ul>
    </article>
  </section>

  <section class="panel" style="margin-top:12px;">
    <h3>Accesos rapidos</h3>
    <div class="acciones-inline" style="margin-top:10px;">
      <button>Nuevo pedido POS</button>
      <button class="secundario">Alta cliente</button>
      <button class="secundario">Renovar membresia</button>
      <button class="secundario">Ver tablero cocina</button>
    </div>
  </section>`
})
export class DashboardComponent {}
