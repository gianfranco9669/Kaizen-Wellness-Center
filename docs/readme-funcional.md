# README funcional

## Operacion actual cubierta

### Seguridad
- Login y refresh token.
- Revocacion de sesiones.
- Recupero de clave (solicitud y confirmacion).
- Bloqueo temporal por intentos fallidos.

### Clientes
- Alta, listado, busqueda, filtros, detalle, edicion y baja logica.
- Campos CRM operativos: estado, VIP, restricciones, notas y saldo.
- Auditoria de cambios.

### Gastronomia
- Alta de pedido con detalle real.
- Cambio de estados para cocina.
- Registro de pagos.
- Consumo de stock por venta.
- Registro de mermas por cancelacion.

### Gimnasio
- Alta/listado/consulta de socios.
- Alta y renovacion de membresias.
- Generacion de QR firmado.
- Check-in validando QR, vencimiento y estado.
- Historial de accesos.

### Integraciones
- Recepcion de webhooks con idempotencia.
- Persistencia y procesamiento asincrono por job.
