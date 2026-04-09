# Kaizen Wellness Center

Base productiva para monolito modular (gastronomia + gimnasio) con ASP.NET Core, Angular, PostgreSQL, Redis, SignalR y Hangfire.

## Requisitos locales
- .NET SDK 8
- Node.js 20+
- Docker + docker-compose

## Estructura
- `src/backend`: API, dominio base y modulos de negocio.
- `src/frontend/wellness-center`: Angular real (standalone components, rutas, guard, interceptor).
- `infraestructura/docker`: despliegue local con postgres + redis + api + frontend.
- `docs`: arquitectura funcional y tecnica.

## Backend
1. `cd src/backend/Api`
2. `dotnet restore`
3. `dotnet build`
4. `dotnet run`

### Credenciales semilla
- usuario: `admin@kaizen.local`
- clave inicial: `Cambiar123*`

### Endpoints base
- Swagger: `http://localhost:8080/swagger`
- Health: `http://localhost:8080/health`
- Hangfire: `http://localhost:8080/hangfire`
- Login: `POST /api/seguridad/auth/login`

## Frontend
1. `cd src/frontend/wellness-center`
2. `npm install`
3. `npm run start`
4. `npm run build`

## Docker
Desde `infraestructura/docker`:
1. `docker compose up --build`
2. API en `http://localhost:8080`
3. Frontend en `http://localhost:4200`

## Seguridad implementada
- JWT real firmado con clave simetrica.
- Refresh token persistido y revocable.
- Bloqueo temporal por intentos fallidos.
- Roles y permisos con politicas de autorizacion.
- Middleware global de errores y trazabilidad en auditoria.

## Modulo clientes implementado
- listado con busqueda y filtros
- alta
- detalle
- edicion
- baja logica
- campos operativos: estado, VIP, restricciones, notas, saldo
- auditoria de alta/edicion/baja

## Cobertura funcional base
- Gastronomia: categorias, productos, pedidos, detalle_pedidos, pagos, stock, mermas, endpoint POS/cocina.
- Gimnasio: socios, membresias, historial de accesos, endpoint recepcion y check-in.
- Integraciones: webhooks con idempotencia + clientes HTTP (PedidosYa, Rappi, MercadoPago) + job de procesamiento.
