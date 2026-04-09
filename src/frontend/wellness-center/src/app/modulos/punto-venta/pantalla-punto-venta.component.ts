import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ServicioPedidos } from './servicio-pedidos';

@Component({
  selector: 'app-punto-venta',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
  <section class="panel">
    <h1>Punto de venta gastronomico</h1>
    <p>Operacion agil para mostrador, take away y delivery.</p>
  </section>

  <section class="bloque-pos" style="margin-top:12px;">
    <article class="panel">
      <h3>Catalogo rapido</h3>
      <div class="grid-productos" style="margin-top:10px;">
        <div class="tarjeta-producto" *ngFor="let producto of productosDemo" (click)="seleccionarProducto(producto.id)">
          <strong>{{ producto.nombre }}</strong>
          <p>{{ producto.categoria }}</p>
          <div class="valor">$ {{ producto.precio | number:'1.0-0' }}</div>
        </div>
      </div>
    </article>

    <article class="panel">
      <h3>Pedido actual</h3>
      <form [formGroup]="formulario" (ngSubmit)="crearPedido()" class="form-columna" style="margin-top:10px;">
        <label>Sede</label><input formControlName="sedeId" />
        <label>Cliente</label><input formControlName="clienteId" />
        <label>Canal</label>
        <select formControlName="canal"><option value="mostrador">Mostrador</option><option value="delivery">Delivery</option></select>
        <label>Producto</label><input formControlName="productoVentaId" />
        <label>Cantidad</label><input type="number" formControlName="cantidad" />
        <label>Observaciones</label><textarea formControlName="observaciones"></textarea>
        <button [disabled]="formulario.invalid">Confirmar pedido</button>
      </form>

      <div *ngIf="pedidoId" style="margin-top:10px;">
        <p><strong>Pedido:</strong> {{ pedidoId }}</p>
        <p><strong>Total:</strong> $ {{ total | number:'1.0-0' }}</p>
        <div class="acciones-inline">
          <button (click)="estado('en-preparacion')">En preparacion</button>
          <button (click)="estado('listo')">Listo</button>
          <button (click)="estado('entregado')">Entregado</button>
        </div>
        <div class="acciones-inline" style="margin-top:8px;">
          <button class="secundario" (click)="pagar('efectivo')">Cobrar efectivo</button>
          <button class="secundario" (click)="pagar('tarjeta')">Cobrar tarjeta</button>
          <button class="secundario" (click)="pagar('billetera')">Cobrar billetera</button>
        </div>
      </div>
    </article>
  </section>`
})
export class PantallaPuntoVentaComponent {
  pedidoId = '';
  total = 0;
  productosDemo = [
    { id: 'SUSHI-001', nombre: 'Roll Salmon Premium', categoria: 'Sushi', precio: 14500 },
    { id: 'SUSHI-002', nombre: 'Nigiri Mix', categoria: 'Sushi', precio: 9900 },
    { id: 'BEB-001', nombre: 'Te verde frio', categoria: 'Bebidas', precio: 2600 }
  ];

  formulario = this.fb.group({
    sedeId: ['', Validators.required],
    clienteId: ['', Validators.required],
    canal: ['mostrador', Validators.required],
    productoVentaId: ['', Validators.required],
    cantidad: [1, [Validators.required, Validators.min(1)]],
    observaciones: ['']
  });

  constructor(private readonly fb: FormBuilder, private readonly servicio: ServicioPedidos) {}

  seleccionarProducto(idProducto: string): void {
    this.formulario.patchValue({ productoVentaId: idProducto });
  }

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
