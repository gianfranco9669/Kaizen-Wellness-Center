import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ServicioClientes, ClienteResumen } from './servicio-clientes';

@Component({
  selector: 'app-lista-clientes',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
  <h1>Clientes</h1>
  <div class="barra-clientes">
    <input [(ngModel)]="busqueda" placeholder="Buscar por nombre, documento o telefono" />
    <select [(ngModel)]="estado">
      <option value="">Todos</option>
      <option value="activo">Activos</option>
      <option value="inactivo">Inactivos</option>
    </select>
    <button (click)="cargar()">Buscar</button>
  </div>

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
  busqueda = '';
  estado = '';
  clientes: ClienteResumen[] = [];

  constructor(private readonly servicio: ServicioClientes) {}
  ngOnInit(): void { this.cargar(); }
  cargar(): void {
    this.servicio.listar(this.busqueda, this.estado).subscribe(data => this.clientes = data);
  }
}
