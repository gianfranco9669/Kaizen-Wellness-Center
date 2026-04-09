import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

interface LoginRequest { email: string; clave: string; }
interface AuthResponse { accessToken: string; refreshToken: string; nombreUsuario: string; }

@Injectable({ providedIn: 'root' })
export class ServicioAutenticacion {
  private readonly claveToken = 'kaizen_access_token';
  private readonly claveRefresh = 'kaizen_refresh_token';
  readonly autenticado = signal<boolean>(!!localStorage.getItem(this.claveToken));

  constructor(private readonly http: HttpClient, private readonly router: Router) {}

  login(email: string, clave: string): Observable<AuthResponse> {
    const payload: LoginRequest = { email, clave };
    return this.http.post<AuthResponse>('/api/seguridad/auth/login', payload).pipe(
      tap((respuesta) => {
        localStorage.setItem(this.claveToken, respuesta.accessToken);
        localStorage.setItem(this.claveRefresh, respuesta.refreshToken);
        this.autenticado.set(true);
      })
    );
  }

  cerrarSesion(): void {
    localStorage.removeItem(this.claveToken);
    localStorage.removeItem(this.claveRefresh);
    this.autenticado.set(false);
    this.router.navigate(['/login']);
  }

  obtenerToken(): string | null {
    return localStorage.getItem(this.claveToken);
  }
}
