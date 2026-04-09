import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SocioResumen { id: string; numeroSocio: string; estadoAcceso: string; fechaVencimientoMembresiaUtc: string; }

@Injectable({ providedIn: 'root' })
export class ServicioGimnasio {
  constructor(private readonly http: HttpClient) {}

  listarSocios(): Observable<SocioResumen[]> {
    return this.http.get<SocioResumen[]>('/api/gimnasio/socios');
  }

  checkin(socioId: string, qrToken?: string): Observable<{ permitido: boolean; mensaje: string }> {
    const query = qrToken ? `?qrToken=${encodeURIComponent(qrToken)}` : '';
    return this.http.post<{ permitido: boolean; mensaje: string }>(`/api/gimnasio/acceso/${socioId}${query}`, {});
  }

  generarQr(socioId: string): Observable<{ qrToken: string; expiraUtc: string }> {
    return this.http.get<{ qrToken: string; expiraUtc: string }>(`/api/gimnasio/socios/${socioId}/qr`);
  }

  historial(socioId: string): Observable<Array<{ fechaAccesoUtc: string; resultado: string }>> {
    return this.http.get<Array<{ fechaAccesoUtc: string; resultado: string }>>(`/api/gimnasio/socios/${socioId}/historial-accesos`);
  }
}
