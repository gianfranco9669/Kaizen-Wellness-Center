import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ServicioClientes } from './servicio-clientes';

@Component({
  selector: 'app-ficha-cliente',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
  <h1>Ficha cliente</h1>
  <form [formGroup]="formulario" (ngSubmit)="guardar()" class="form-cliente">
    <label>Nombres</label><input formControlName="nombres" />
    <label>Apellidos</label><input formControlName="apellidos" />
    <label>Telefono</label><input formControlName="telefono" />
    <label>Email</label><input formControlName="email" />
    <label>Estado</label><input formControlName="estado" />
    <label>Saldo</label><input type="number" formControlName="saldoCuenta" />
    <label>Restricciones alimentarias</label><textarea formControlName="restriccionesAlimentarias"></textarea>
    <label>Notas internas</label><textarea formControlName="notasInternas"></textarea>
    <button type="submit">Guardar cambios</button>
    <button type="button" (click)="eliminar()">Baja logica</button>
  </form>`
})
export class FichaClienteComponent implements OnInit {
  private clienteId = '';
  formulario = this.fb.group({
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
    this.servicio.obtener(this.clienteId).subscribe(c => this.formulario.patchValue(c));
  }

  guardar(): void {
    if (this.formulario.invalid) return;
    this.servicio.actualizar(this.clienteId, this.formulario.getRawValue()).subscribe();
  }

  eliminar(): void {
    this.servicio.eliminar(this.clienteId).subscribe(() => this.router.navigate(['/clientes']));
  }
}
