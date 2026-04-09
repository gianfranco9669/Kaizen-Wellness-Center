import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClienteDetalle, ServicioClientes } from './servicio-clientes';

@Component({
  selector: 'app-ficha-cliente',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
  <section class="panel">
    <h1>Ficha cliente</h1>
    <p>Vista integral de datos personales, estado comercial y notas operativas.</p>

    <form [formGroup]="formulario" (ngSubmit)="guardar()" class="form-columna" style="margin-top:12px;">
      <div class="form-grid">
        <div class="form-columna">
          <label>Nombres</label><input formControlName="nombres" />
          <label>Apellidos</label><input formControlName="apellidos" />
          <label>Telefono</label><input formControlName="telefono" />
          <label>Email</label><input formControlName="email" />
        </div>
        <div class="form-columna">
          <label>Estado</label><input formControlName="estado" />
          <label>Saldo</label><input type="number" formControlName="saldoCuenta" />
          <label>Segmento VIP</label>
          <select formControlName="esVip">
            <option [ngValue]="false">Regular</option>
            <option [ngValue]="true">VIP</option>
          </select>
        </div>
      </div>

      <div class="form-grid">
        <div class="form-columna">
          <label>Restricciones alimentarias</label><textarea formControlName="restriccionesAlimentarias"></textarea>
        </div>
        <div class="form-columna">
          <label>Notas internas</label><textarea formControlName="notasInternas"></textarea>
        </div>
      </div>

      <div class="acciones-inline">
        <button type="submit">Guardar cambios</button>
        <button type="button" class="secundario" (click)="eliminar()">Baja logica</button>
      </div>
    </form>
  </section>`
})
export class FichaClienteComponent implements OnInit {
  private clienteId = '';

  formulario = this.fb.nonNullable.group({
    nombres: ['', Validators.required],
    apellidos: ['', Validators.required],
    telefono: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    estado: ['activo', Validators.required],
    esVip: [false],
    saldoCuenta: [0],
    restriccionesAlimentarias: [''],
    notasInternas: ['']
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly ruta: ActivatedRoute,
    private readonly router: Router,
    private readonly servicio: ServicioClientes) {}

  ngOnInit(): void {
    this.clienteId = this.ruta.snapshot.paramMap.get('id') ?? '';
    this.servicio.obtener(this.clienteId).subscribe(c => {
      this.formulario.patchValue({
        nombres: c.nombres,
        apellidos: c.apellidos,
        telefono: c.telefono,
        email: c.email,
        estado: c.estado,
        esVip: c.esVip,
        saldoCuenta: c.saldoCuenta,
        restriccionesAlimentarias: c.restriccionesAlimentarias,
        notasInternas: c.notasInternas
      });
    });
  }

  guardar(): void {
    if (this.formulario.invalid) return;

    const payload: Partial<ClienteDetalle> = {
      ...this.formulario.getRawValue(),
      saldoCuenta: Number(this.formulario.controls.saldoCuenta.value)
    };

    this.servicio.actualizar(this.clienteId, payload).subscribe();
  }

  eliminar(): void {
    this.servicio.eliminar(this.clienteId).subscribe(() => this.router.navigate(['/clientes']));
  }
}
