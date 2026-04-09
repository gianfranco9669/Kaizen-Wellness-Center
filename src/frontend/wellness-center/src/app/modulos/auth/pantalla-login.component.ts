import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ServicioAutenticacion } from '../../nucleo/servicios/servicio-autenticacion';

@Component({
  selector: 'app-pantalla-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
  <main style="min-height:100vh; display:grid; place-items:center; padding:20px;">
    <section class="panel" style="max-width:430px; width:100%;">
      <h1>Kaizen Wellness Center</h1>
      <p>Gestion premium para gastronomia y gimnasio.</p>
      <form [formGroup]="formulario" (ngSubmit)="ingresar()" class="form-columna" style="margin-top:12px;">
        <label>Email</label>
        <input type="email" formControlName="email" placeholder="admin@kaizen.local" />
        <label>Clave</label>
        <input type="password" formControlName="clave" placeholder="********" />
        <button type="submit" [disabled]="formulario.invalid || cargando">Ingresar al sistema</button>
      </form>
      <p class="pastilla error" *ngIf="error" style="margin-top:10px;">{{ error }}</p>
    </section>
  </main>
  `
})
export class PantallaLoginComponent {
  error = '';
  cargando = false;
  formulario = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    clave: ['', [Validators.required, Validators.minLength(8)]]
  });

  constructor(private readonly fb: FormBuilder, private readonly auth: ServicioAutenticacion, private readonly router: Router) {}

  ingresar(): void {
    if (this.formulario.invalid) return;
    this.cargando = true;
    const { email, clave } = this.formulario.getRawValue() as { email: string; clave: string };
    this.auth.login(email, clave).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => {
        this.error = 'Credenciales invalidas';
        this.cargando = false;
      }
    });
  }
}
