import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServicioGimnasio, SocioResumen } from './servicio-gimnasio';

@Component({
  selector: 'app-pantalla-recepcion-gimnasio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <h1>Recepcion gimnasio</h1>
  <div class="tarjeta">
    <label>Socio</label>
    <select [(ngModel)]="socioIdSeleccionado">
      <option *ngFor="let s of socios" [ngValue]="s.id">{{ s.numeroSocio }} - {{ s.estadoAcceso }}</option>
    </select>
    <button (click)="generarQr()">Generar QR</button>
    <textarea [(ngModel)]="qrToken" placeholder="Pegar token QR"></textarea>
    <button (click)="checkin()">Registrar check-in</button>
    <p>{{ mensaje }}</p>
  </div>
  <section class="tarjeta" *ngIf="historial.length">
    <h3>Historial accesos</h3>
    <table class="tabla"><tr><th>Fecha</th><th>Resultado</th></tr>
      <tr *ngFor="let h of historial"><td>{{ h.fechaAccesoUtc }}</td><td>{{ h.resultado }}</td></tr>
    </table>
  </section>`
})
export class PantallaRecepcionGimnasioComponent {
  socios: SocioResumen[] = [];
  socioIdSeleccionado = '';
  qrToken = '';
  mensaje = '';
  historial: Array<{ fechaAccesoUtc: string; resultado: string }> = [];

  constructor(private readonly servicio: ServicioGimnasio) {
    this.servicio.listarSocios().subscribe(data => {
      this.socios = data;
      this.socioIdSeleccionado = data[0]?.id ?? '';
    });
  }

  generarQr(): void {
    if (!this.socioIdSeleccionado) return;
    this.servicio.generarQr(this.socioIdSeleccionado).subscribe(r => this.qrToken = r.qrToken);
  }

  checkin(): void {
    if (!this.socioIdSeleccionado) return;
    this.servicio.checkin(this.socioIdSeleccionado, this.qrToken).subscribe(r => {
      this.mensaje = r.mensaje;
      this.servicio.historial(this.socioIdSeleccionado).subscribe(h => this.historial = h);
    });
  }
}
