import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ServicioClientes, ClienteResumen } from './servicio-clientes';

@Component({
  selector: 'app-lista-clientes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
  <section class="panel">
    <h1>Clientes y CRM operativo</h1>
    <p>Gestion comercial con filtros, segmentacion VIP y ficha detallada.</p>

    <form [formGroup]="formFiltro" class="form-grid" style="margin-top:12px;" (ngSubmit)="cargar()">
      <input formControlName="busqueda" placeholder="Buscar por nombre, documento o telefono" />
      <select formControlName="estado">
        <option value="">Todos los estados</option>
        <option value="activo">Activo</option>
        <option value="inactivo">Inactivo</option>
      </select>
      <button type="submit">Aplicar filtros</button>
      <button type="button" class="secundario" (click)="limpiarFiltros()">Limpiar</button>
    </form>
  </section>

  <section class="grid-dos" style="margin-top:12px;">
    <article class="panel">
      <h3>Alta rapida de cliente</h3>
      <form [formGroup]="formAlta" (ngSubmit)="crear()" class="form-columna" style="margin-top:10px;">
        <div class="form-grid">
          <input formControlName="nombres" placeholder="Nombres" />
          <input formControlName="apellidos" placeholder="Apellidos" />
          <input formControlName="documento" placeholder="Documento" />
          <input formControlName="telefono" placeholder="Telefono" />
          <input formControlName="email" placeholder="Email" />
          <select formControlName="esVip">
            <option [ngValue]="false">Cliente regular</option>
            <option [ngValue]="true">Cliente VIP</option>
          </select>
        </div>
        <button [disabled]="formAlta.invalid">Crear cliente</button>
      </form>
    </article>

    <article class="panel">
      <h3>Indicadores clientes</h3>
      <div class="grid-kpi" style="margin-top:10px;">
        <div class="kpi"><div>Total activos</div><div class="valor">{{ clientes.length }}</div></div>
        <div class="kpi"><div>VIP</div><div class="valor">{{ contarVip() }}</div></div>
        <div class="kpi"><div>Saldo acumulado</div><div class="valor">{{ calcularSaldo() | number:'1.0-0' }}</div></div>
      </div>
    </article>
  </section>

  <section class="panel" style="margin-top:12px;">
    <h3>Listado</h3>
    <table class="tabla" style="margin-top:10px;">
      <thead>
        <tr>
          <th>Codigo</th>
          <th>Nombre</th>
          <th>Documento</th>
          <th>Telefono</th>
          <th>Segmento</th>
          <th>Estado</th>
          <th>Saldo</th>
          <th>Accion</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let c of clientes">
          <td>{{ c.codigoCliente }}</td>
          <td>{{ c.nombreCompleto }}</td>
          <td>{{ c.documento }}</td>
          <td>{{ c.telefono }}</td>
          <td><span class="pastilla" [class.ok]="c.esVip" [class.alerta]="!c.esVip">{{ c.esVip ? 'VIP' : 'Regular' }}</span></td>
          <td>{{ c.estado }}</td>
          <td>{{ c.saldoCuenta | number:'1.2-2' }}</td>
          <td><a [routerLink]="['/clientes', c.id]">Abrir ficha</a></td>
        </tr>
      </tbody>
    </table>
  </section>`
})
export class ListaClientesComponent implements OnInit {
  clientes: ClienteResumen[] = [];

  formFiltro = this.fb.nonNullable.group({
    busqueda: '',
    estado: ''
  });

  formAlta = this.fb.nonNullable.group({
    nombres: ['', Validators.required],
    apellidos: ['', Validators.required],
    documento: ['', Validators.required],
    telefono: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    esVip: false
  });

  constructor(private readonly fb: FormBuilder, private readonly servicio: ServicioClientes) {}

  ngOnInit(): void { this.cargar(); }

  cargar(): void {
    const raw = this.formFiltro.getRawValue();
    this.servicio.listar(raw.busqueda, raw.estado).subscribe(data => this.clientes = data);
  }

  limpiarFiltros(): void {
    this.formFiltro.setValue({ busqueda: '', estado: '' });
    this.cargar();
  }

  crear(): void {
    if (this.formAlta.invalid) return;
    const raw = this.formAlta.getRawValue();
    this.servicio.crear({
      nombres: raw.nombres,
      apellidos: raw.apellidos,
      documento: raw.documento,
      telefono: raw.telefono,
      email: raw.email,
      esVip: raw.esVip,
      restriccionesAlimentarias: '',
      notasInternas: ''
    }).subscribe(() => {
      this.formAlta.reset({ nombres: '', apellidos: '', documento: '', telefono: '', email: '', esVip: false });
      this.cargar();
    });
  }

  contarVip(): number {
    return this.clientes.filter(x => x.esVip).length;
  }

  calcularSaldo(): number {
    return this.clientes.reduce((acc, x) => acc + x.saldoCuenta, 0);
  }
}
