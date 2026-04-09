# Kaizen Wellness Center

Plataforma premium para operar un wellness center multiunidad (gastronomia + gimnasio + crecimiento futuro) con monolito modular robusto.

## Etapas implementadas en esta base
- Etapa 1: arquitectura, estructura de carpetas, modelado inicial.
- Etapa 2: backend base ASP.NET Core, PostgreSQL, Redis, SignalR, jobs.
- Etapa 3: frontend base Angular (estructura y pantallas semilla).
- Etapa 4-6: esqueletos de modulos para evolucion incremental.
- Etapa 7: documentacion y checklist de produccion inicial.

## Estructura
- `src/backend`: solucion .NET con API, comunes y modulos.
- `src/frontend/wellness-center`: base frontend Angular y experiencia UX inicial.
- `infraestructura/docker`: contenedores para despliegue local/entornos.
- `docs`: decisiones tecnicas y arquitectura.

## Backend destacado
- Autenticacion base con login y bloqueo por intentos fallidos.
- CRM de clientes inicial con auditoria y estados.
- Preparado para SignalR (cocina), Redis y Hangfire.
- Migracion SQL inicial y semilla de datos empresariales.

## Frontend destacado
- Shell inicial con experiencias: admin, punto de venta, cocina, recepcion gimnasio y portal socio.
- Textos en castellano, nomenclatura sin tildes ni letra enie en codigo.

## Checklist de produccion (inicial)
- [ ] JWT real con firma segura y refresh token persistente.
- [ ] Integrar Identity + 2FA.
- [ ] Endurecer politicas CORS por entorno.
- [ ] CI/CD con pruebas automatizadas.
- [ ] Observabilidad OpenTelemetry + panel centralizado.
- [ ] Integraciones reales con Mercado Pago, PedidosYa y Rappi.
