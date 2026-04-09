import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ServicioPedidos } from './servicio-pedidos';

@Component({
  selector: 'app-punto-venta',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
  <h1>Punto de venta gastronomico</h1>
  <form [formGroup]="formulario" (ngSubmit)="crearPedido()" class="tarjeta">
    <label>Sede</label><input formControlName="sedeId" />
    <label>Cliente</label><input formControlName="clienteId" />
    <label>Canal</label>
    <select formControlName="canal"><option value="mostrador">Mostrador</option><option value="delivery">Delivery</option></select>
    <label>Producto</label><input formControlName="productoVentaId" />
    <label>Cantidad</label><input type="number" formControlName="cantidad" />
    <label>Observaciones</label><textarea formControlName="observaciones"></textarea>
    <button [disabled]="formulario.invalid">Crear pedido</button>
  </form>

  <section class="tarjeta" *ngIf="pedidoId">
    <h3>Pedido creado: {{ pedidoId }}</h3>
    <div class="acciones-inline">
      <button (click)="estado('en-preparacion')">En preparacion</button>
      <button (click)="estado('listo')">Listo</button>
      <button (click)="estado('entregado')">Entregado</button>
    </div>
    <div class="acciones-inline">
      <button (click)="pagar('efectivo')">Cobrar efectivo</button>
      <button (click)="pagar('tarjeta')">Cobrar tarjeta</button>
    </div>
  </section>`
})
export class PantallaPuntoVentaComponent {
  pedidoId = '';
  total = 0;
  formulario = this.fb.group({
    sedeId: ['', Validators.required],
    clienteId: ['', Validators.required],
    canal: ['mostrador', Validators.required],
    productoVentaId: ['', Validators.required],
    cantidad: [1, [Validators.required, Validators.min(1)]],
    observaciones: ['']
  });

  constructor(private readonly fb: FormBuilder, private readonly servicio: ServicioPedidos) {}

  crearPedido(): void {
    const raw = this.formulario.getRawValue();
    this.servicio.crearPedido({
      sedeId: raw.sedeId!,
      clienteId: raw.clienteId!,
      canal: raw.canal!,
      observaciones: raw.observaciones ?? '',
      items: [{ productoVentaId: raw.productoVentaId!, cantidad: Number(raw.cantidad ?? 1) }]
    }).subscribe((pedido) => {
      this.pedidoId = pedido.id;
      this.total = pedido.total;
    });
  }

  estado(estado: string): void {
    if (!this.pedidoId) return;
    this.servicio.cambiarEstado(this.pedidoId, estado).subscribe();
  }

  pagar(medioPago: string): void {
    if (!this.pedidoId) return;
    this.servicio.registrarPago({ pedidoId: this.pedidoId, medioPago, importe: this.total }).subscribe();
  }
}
