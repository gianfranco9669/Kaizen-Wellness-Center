import { Component } from '@angular/core';

@Component({
  selector: 'app-pantalla-recepcion-gimnasio',
  standalone: true,
  template: `
  <h1>Recepcion gimnasio</h1>
  <div class="tarjeta">
    <p>Escaneo QR y validacion inmediata de membresia.</p>
    <button>Simular check-in</button>
  </div>`
})
export class PantallaRecepcionGimnasioComponent {}
