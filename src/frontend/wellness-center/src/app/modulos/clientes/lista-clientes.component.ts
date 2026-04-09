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
  <h1>Clientes</h1>
  <form [formGroup]="formFiltro" class="barra-clientes" (ngSubmit)="cargar()">
    <input formControlName="busqueda" placeholder="Buscar por nombre, documento o telefono" />
    <select formControlName="estado">
      <option value="">Todos</option>
      <option value="activo">Activos</option>
      <option value="inactivo">Inactivos</option>
    </select>
    <button type="submit">Buscar</button>
  </form>

  <section class="tarjeta">
    <h3>Alta rapida</h3>
    <form [formGroup]="formAlta" (ngSubmit)="crear()">
      <div class="grid-kpi">
        <input formControlName="nombres" placeholder="Nombres" />
        <input formControlName="apellidos" placeholder="Apellidos" />
        <input formControlName="documento" placeholder="Documento" />
        <input formControlName="telefono" placeholder="Telefono" />
        <input formControlName="email" placeholder="Email" />
        <button [disabled]="formAlta.invalid">Crear cliente</button>
      </div>
    </form>
  </section>

  <table class="tabla">
    <thead><tr><th>Codigo</th><th>Nombre</th><th>Documento</th><th>Telefono</th><th>VIP</th><th>Saldo</th><th></th></tr></thead>
    <tbody>
      <tr *ngFor="let c of clientes">
        <td>{{ c.codigoCliente }}</td>
        <td>{{ c.nombreCompleto }}</td>
        <td>{{ c.documento }}</td>
        <td>{{ c.telefono }}</td>
        <td>{{ c.esVip ? 'Si' : 'No' }}</td>
        <td>{{ c.saldoCuenta | number:'1.2-2' }}</td>
        <td><a [routerLink]="['/clientes', c.id]">Ficha</a></td>
      </tr>
    </tbody>
  </table>`
})
export class ListaClientesComponent implements OnInit {
  clientes: ClienteResumen[] = [];

  formFiltro = this.fb.group({
    busqueda: [''],
    estado: ['']
  });

  formAlta = this.fb.group({
    nombres: ['', Validators.required],
    apellidos: ['', Validators.required],
    documento: ['', Validators.required],
    telefono: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]]
  });

  constructor(private readonly fb: FormBuilder, private readonly servicio: ServicioClientes) {}

  ngOnInit(): void { this.cargar(); }

  cargar(): void {
    const raw = this.formFiltro.getRawValue();
    this.servicio.listar(raw.busqueda ?? '', raw.estado ?? '').subscribe(data => this.clientes = data);
  }

  crear(): void {
    if (this.formAlta.invalid) return;
    const raw = this.formAlta.getRawValue();
    this.servicio.crear({
      nombres: raw.nombres ?? '',
      apellidos: raw.apellidos ?? '',
      documento: raw.documento ?? '',
      telefono: raw.telefono ?? '',
      email: raw.email ?? '',
      esVip: false,
      restriccionesAlimentarias: '',
      notasInternas: ''
    }).subscribe(() => {
      this.formAlta.reset();
      this.cargar();
    });
  }
}
