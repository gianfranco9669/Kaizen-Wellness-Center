import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServicioGimnasio } from '../recepcion-gimnasio/servicio-gimnasio';

@Component({
  selector: 'app-portal-socio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <h1>Portal socio</h1>
  <div class="tarjeta">
    <label>ID socio</label>
    <input [(ngModel)]="socioId" />
    <button (click)="cargarQr()">Generar QR personal</button>
    <p *ngIf="expiraUtc">Expira: {{ expiraUtc }}</p>
    <textarea [value]="qrToken" readonly rows="4"></textarea>
  </div>
  <div class="tarjeta" *ngIf="historial.length">
    <h3>Ultimos accesos</h3>
    <ul><li *ngFor="let h of historial">{{ h.fechaAccesoUtc }} - {{ h.resultado }}</li></ul>
  </div>`
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
