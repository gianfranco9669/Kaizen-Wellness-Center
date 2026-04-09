import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServicioGimnasio, SocioResumen } from './servicio-gimnasio';

@Component({
  selector: 'app-pantalla-recepcion-gimnasio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <section class="panel">
    <h1>Recepcion gimnasio</h1>
    <p>Check-in rapido con validacion de membresia y QR.</p>
  </section>

  <section class="grid-dos" style="margin-top:12px;">
    <article class="panel">
      <h3>Control de acceso</h3>
      <div class="form-columna" style="margin-top:10px;">
        <label>Socio</label>
        <select [(ngModel)]="socioIdSeleccionado">
          <option *ngFor="let s of socios" [ngValue]="s.id">{{ s.numeroSocio }} · {{ s.estadoAcceso }}</option>
        </select>
        <div class="acciones-inline">
          <button (click)="generarQr()">Generar QR</button>
          <button class="secundario" (click)="checkin()">Registrar check-in</button>
        </div>
        <label>Token QR</label>
        <textarea [(ngModel)]="qrToken" placeholder="Escanear o pegar token QR"></textarea>
      </div>
      <div style="margin-top:10px;">
        <span class="pastilla" [class.ok]="mensaje.includes('habilitado')" [class.error]="mensaje.includes('bloqueado')">{{ mensaje || 'Sin validacion' }}</span>
      </div>
    </article>

    <article class="panel">
      <h3>Estado rapido</h3>
      <ul class="lista-simple" style="margin-top:10px;">
        <li class="item-simple">Socios cargados: {{ socios.length }}</li>
        <li class="item-simple">Ultimo QR: {{ qrToken ? 'generado' : 'sin generar' }}</li>
        <li class="item-simple">Historial visible: {{ historial.length }} registros</li>
      </ul>
    </article>
  </section>

  <section class="panel" style="margin-top:12px;" *ngIf="historial.length">
    <h3>Historial de accesos</h3>
    <table class="tabla" style="margin-top:10px;"><tr><th>Fecha</th><th>Resultado</th></tr>
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
