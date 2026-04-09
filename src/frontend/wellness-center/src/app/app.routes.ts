import { Routes } from '@angular/router';
import { PantallaLoginComponent } from './modulos/auth/pantalla-login.component';
import { LayoutAdministrativoComponent } from './modulos/admin/layout-administrativo.component';
import { DashboardComponent } from './modulos/admin/dashboard.component';
import { ListaClientesComponent } from './modulos/clientes/lista-clientes.component';
import { FichaClienteComponent } from './modulos/clientes/ficha-cliente.component';
import { PantallaPuntoVentaComponent } from './modulos/punto-venta/pantalla-punto-venta.component';
import { PantallaCocinaComponent } from './modulos/cocina/pantalla-cocina.component';
import { PantallaRecepcionGimnasioComponent } from './modulos/recepcion-gimnasio/pantalla-recepcion-gimnasio.component';
import { PortalSocioComponent } from './modulos/portal-socio/portal-socio.component';
import { guardAutenticado } from './nucleo/guards/autenticado.guard';

export const rutasAplicacion: Routes = [
  { path: 'login', component: PantallaLoginComponent },
  {
    path: '',
    component: LayoutAdministrativoComponent,
    canActivate: [guardAutenticado],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'clientes', component: ListaClientesComponent },
      { path: 'clientes/:id', component: FichaClienteComponent },
      { path: 'punto-venta', component: PantallaPuntoVentaComponent },
      { path: 'cocina-tiempo-real', component: PantallaCocinaComponent },
      { path: 'recepcion-gimnasio', component: PantallaRecepcionGimnasioComponent },
      { path: 'portal-socio', component: PortalSocioComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];
