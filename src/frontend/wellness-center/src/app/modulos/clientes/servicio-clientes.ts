import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ClienteResumen {
  id: string;
  codigoCliente: string;
  nombreCompleto: string;
  documento: string;
  telefono: string;
  esVip: boolean;
  saldoCuenta: number;
  estado: string;
}

export interface ClienteDetalle {
  id: string;
  codigoCliente: string;
  nombres: string;
  apellidos: string;
  documento: string;
  telefono: string;
  email: string;
  esVip: boolean;
  saldoCuenta: number;
  estado: string;
  restriccionesAlimentarias: string;
  notasInternas: string;
  fechaCreacionUtc: string;
}

@Injectable({ providedIn: 'root' })
export class ServicioClientes {
  private readonly empresaIdDemo = '11111111-1111-1111-1111-111111111111';

  constructor(private readonly http: HttpClient) {}

  listar(busqueda = '', estado = '', esVip?: boolean): Observable<ClienteResumen[]> {
    let params = new HttpParams();
    if (busqueda) params = params.set('busqueda', busqueda);
    if (estado) params = params.set('estado', estado);
    if (esVip !== undefined) params = params.set('esVip', esVip);
    return this.http.get<ClienteResumen[]>(`/api/clientes/empresa/${this.empresaIdDemo}`, { params });
  }

  obtener(id: string): Observable<ClienteDetalle> {
    return this.http.get<ClienteDetalle>(`/api/clientes/${id}`);
  }

  crear(payload: Partial<ClienteDetalle>): Observable<ClienteResumen> {
    return this.http.post<ClienteResumen>('/api/clientes', {
      empresaId: this.empresaIdDemo,
      nombres: payload.nombres,
      apellidos: payload.apellidos,
      documento: payload.documento,
      telefono: payload.telefono,
      email: payload.email,
      esVip: payload.esVip,
      restriccionesAlimentarias: payload.restriccionesAlimentarias ?? '',
      notasInternas: payload.notasInternas ?? ''
    });
  }

  actualizar(id: string, payload: Partial<ClienteDetalle>): Observable<ClienteDetalle> {
    return this.http.put<ClienteDetalle>(`/api/clientes/${id}`, payload);
  }

  eliminar(id: string): Observable<void> {
    return this.http.delete<void>(`/api/clientes/${id}`);
  }
}
