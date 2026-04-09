# Arquitectura general - Kaizen Wellness Center

## Vision
Monolito modular en ASP.NET Core + Angular, preparado para evolucionar a servicios independientes por modulo sin romper contratos.

## Capas
- Dominio: entidades, reglas invariantes, eventos de dominio.
- Aplicacion: casos de uso, DTOs, validadores, autorizacion por politica.
- Infraestructura: EF Core PostgreSQL, Redis, integraciones HTTP, webhooks, outbox.
- API: controladores, middleware, SignalR, OpenAPI, rate limiting.
- Frontend Angular: shell admin, POS, cocina, recepcion gimnasio, portal socio PWA.
- Jobs: tareas recurrentes (sincronizacion, alertas, limpieza).
- Observabilidad: logs estructurados, health checks, trazas y metricas.

## Modulos base
1. seguridad
2. clientes
3. gastronomia
4. inventario
5. compras
6. finanzas
7. facturacion
8. gimnasio
9. integraciones
10. reportes
11. configuracion
12. auditoria

## Dominio textual
- Empresa 1..N Sede
- Empresa 1..N Cliente
- Cliente 1..0..1 SocioGimnasio
- SocioGimnasio 1..N Membresia
- Sede 1..N Pedido
- Pedido N..N ProductoVenta (detalle pendiente en etapa 4)
- ProductoVenta 1..N MovimientoStock
- Proveedor 1..N OrdenCompra (etapa 4)
- CajaDiaria 1..N MovimientoCaja (etapa 4)

## Estrategia de crecimiento
- Contratos internos por modulo.
- Eventos de integracion persistidos en outbox.
- Idempotencia para webhooks de terceros.
- Clave de particion por empresa/sede para futura multiempresa.
