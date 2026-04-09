import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-pantalla-cocina',
  standalone: true,
  imports: [CommonModule],
  template: `
  <section class="panel">
    <h1>Cocina tiempo real</h1>
    <p>Vista operativa por estados para produccion y despacho.</p>
    <div class="acciones-inline" style="margin-top:10px;">
      <button (click)="cargar()">Actualizar tablero</button>
      <button class="secundario" (click)="filtrar = 'pendiente'">Pendientes</button>
      <button class="secundario" (click)="filtrar = 'en-preparacion'">En preparacion</button>
      <button class="secundario" (click)="filtrar = ''">Todos</button>
    </div>
  </section>

  <section class="grid-kpi" style="margin-top:12px;" *ngIf="pedidosFiltrados().length">
    <article class="panel" *ngFor="let p of pedidosFiltrados()">
      <div class="acciones-inline" style="justify-content:space-between;">
        <strong>{{ p.id }}</strong>
        <span class="pastilla" [class.alerta]="p.estado === 'pendiente'" [class.ok]="p.estado !== 'pendiente'">{{ p.estado }}</span>
      </div>
      <p style="margin-top:8px;">Canal: {{ p.canal }}</p>
      <p>Total: $ {{ p.total | number:'1.0-0' }}</p>
      <p>Obs: {{ p.observaciones || 'Sin observaciones' }}</p>
    </article>
  </section>`
})
export class PantallaCocinaComponent {
  pedidos: Array<{ id: string; canal: string; estado: string; total: number; observaciones: string }> = [];
  filtrar = '';
  private readonly sedeDemo = '11111111-1111-1111-1111-111111111111';

  constructor(private readonly http: HttpClient) { this.cargar(); }

  cargar(): void {
    this.http.get<Array<{ id: string; canal: string; estado: string; total: number; observaciones: string }>>(`/api/gastronomia/pedidos/sede/${this.sedeDemo}`)
      .subscribe(data => this.pedidos = data);
  }

  pedidosFiltrados(): Array<{ id: string; canal: string; estado: string; total: number; observaciones: string }> {
    if (!this.filtrar) return this.pedidos;
    return this.pedidos.filter(p => p.estado === this.filtrar);
  }
}
