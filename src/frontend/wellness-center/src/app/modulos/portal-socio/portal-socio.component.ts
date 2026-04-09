import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServicioGimnasio } from '../recepcion-gimnasio/servicio-gimnasio';

@Component({
  selector: 'app-portal-socio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <section class="panel">
    <h1>Portal socio</h1>
    <p>Experiencia premium para membresia, accesos y beneficios.</p>
  </section>

  <section class="grid-dos" style="margin-top:12px;">
    <article class="panel">
      <h3>Mi acceso QR</h3>
      <div class="form-columna" style="margin-top:10px;">
        <label>ID socio</label>
        <input [(ngModel)]="socioId" placeholder="Ingresar ID socio" />
        <button (click)="cargarQr()">Generar QR personal</button>
        <p *ngIf="expiraUtc">Expira: {{ expiraUtc }}</p>
        <textarea [value]="qrToken" readonly rows="4"></textarea>
      </div>
    </article>

    <article class="panel">
      <h3>Resumen membresia</h3>
      <ul class="lista-simple" style="margin-top:10px;">
        <li class="item-simple">Plan actual: Premium Integral</li>
        <li class="item-simple">Vencimiento estimado: {{ expiraUtc || 'Sin datos' }}</li>
        <li class="item-simple">Beneficios activos: Spa, Gym, Gastronomia</li>
      </ul>
    </article>
  </section>

  <section class="panel" style="margin-top:12px;" *ngIf="historial.length">
    <h3>Ultimos accesos</h3>
    <ul class="lista-simple" style="margin-top:10px;"><li class="item-simple" *ngFor="let h of historial">{{ h.fechaAccesoUtc }} · {{ h.resultado }}</li></ul>
  </section>`
})
export class PortalSocioComponent {
  socioId = '';
  qrToken = '';
  expiraUtc = '';
  historial: Array<{ fechaAccesoUtc: string; resultado: string }> = [];

  constructor(private readonly servicio: ServicioGimnasio) {}

  cargarQr(): void {
    if (!this.socioId) return;
    this.servicio.generarQr(this.socioId).subscribe(r => {
      this.qrToken = r.qrToken;
      this.expiraUtc = r.expiraUtc;
      this.servicio.historial(this.socioId).subscribe(h => this.historial = h);
    });
  }
}
