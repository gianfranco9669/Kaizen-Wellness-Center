import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ItemPedido { productoVentaId: string; cantidad: number; }
export interface PedidoCreado { id: string; estado: string; total: number; }

@Injectable({ providedIn: 'root' })
export class ServicioPedidos {
  constructor(private readonly http: HttpClient) {}

  crearPedido(payload: { sedeId: string; clienteId: string; canal: string; observaciones: string; items: ItemPedido[] }): Observable<PedidoCreado> {
    return this.http.post<PedidoCreado>('/api/gastronomia/pedidos', payload);
  }

  cambiarEstado(pedidoId: string, estado: string): Observable<void> {
    return this.http.put<void>(`/api/gastronomia/pedidos/${pedidoId}/estado/${estado}`, {});
  }

  registrarPago(payload: { pedidoId: string; medioPago: string; importe: number }): Observable<unknown> {
    return this.http.post('/api/gastronomia/pagos', payload);
  }
}
