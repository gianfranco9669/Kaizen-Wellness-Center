import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-pantalla-cocina',
  standalone: true,
  imports: [CommonModule],
  template: `
  <h1>Cocina tiempo real</h1>
  <button (click)="cargar()">Actualizar tablero</button>
  <div class="grid-kpi" *ngIf="pedidos.length">
    <article class="tarjeta" *ngFor="let p of pedidos">
      <h3>{{ p.id }}</h3>
      <p>Canal: {{ p.canal }}</p>
      <p>Estado: {{ p.estado }}</p>
      <p>Total: {{ p.total | number:'1.2-2' }}</p>
    </article>
  </div>`
})
export class PantallaCocinaComponent {
  pedidos: Array<{ id: string; canal: string; estado: string; total: number }> = [];
  private readonly sedeDemo = '11111111-1111-1111-1111-111111111111';

  constructor(private readonly http: HttpClient) { this.cargar(); }

  cargar(): void {
    this.http.get<Array<{ id: string; canal: string; estado: string; total: number }>>(`/api/gastronomia/pedidos/sede/${this.sedeDemo}`)
      .subscribe(data => this.pedidos = data);
  }
}
