# README funcional

## Experiencias de uso
- Panel administrativo: dashboard, clientes, monitoreo operativo.
- Punto de venta: base de operacion gastronomica.
- Cocina: vista de estado de pedidos.
- Recepcion gimnasio: control de acceso y check-in.
- Portal socio: estado de plan y accesos.

## Modulo clientes (operativo/comercial)
- Ficha integral del cliente.
- Estados activo/inactivo y baja logica.
- Segmentacion VIP.
- Restricciones alimentarias y notas internas.
- Saldo de cuenta.
- Base para historiales de pedidos, pagos, membresias y asistencia.

## Integraciones
- Recepcion de webhooks con idempotencia.
- Estados de sincronizacion y reintentos por job.
- Clientes HTTP desacoplados para PedidosYa, Rappi y MercadoPago.

## Seguridad
- Login + refresh token.
- Bloqueo temporal por intentos fallidos.
- Endpoints protegidos con JWT.
- Politicas por permisos para acciones sensibles.
