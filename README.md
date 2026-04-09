# Kaizen Wellness Center

Base productiva para monolito modular (gastronomia + gimnasio) con ASP.NET Core, Angular, PostgreSQL, Redis, SignalR y Hangfire.

## Requisitos locales
- .NET SDK 8
- Node.js 20+
- Docker + docker-compose

## CI integrado
- Backend: `.github/workflows/ci-backend.yml` (restore/build/test).
- Frontend: `.github/workflows/ci-frontend.yml` (install/build/test).
- Los PRs ejecutan checks reales para validar compilacion y pruebas.

## Variables de entorno
1. Copiar `infraestructura/docker/.env.ejemplo` a `infraestructura/docker/.env`.
2. Configurar secretos (`POSTGRES_PASSWORD`, `REDIS_PASSWORD`, `JWT_CLAVE_SECRETA`, `QR_CLAVE_SECRETA`).
3. No dejar secretos en `appsettings.json` ni en `docker-compose`.

## Estructura
- `src/backend`: API, dominio y modulos.
- `src/frontend/wellness-center`: Angular real (rutas, guard, interceptor, formularios).
- `infraestructura/docker`: despliegue local con postgres + redis + api + frontend.
- `tests/backend/Api.UnitTests`: pruebas unitarias backend.
- `docs`: arquitectura funcional y tecnica.

## Backend
1. `cd src/backend/Api`
2. Exportar variables de entorno:
   - `ConnectionStrings__PostgreSql`
   - `ConnectionStrings__Redis`
   - `Jwt__ClaveSecreta`
   - `Qr__ClaveSecreta`
3. `dotnet restore`
4. `dotnet build`
5. `dotnet run`

### Credenciales semilla
- usuario: `admin@kaizen.local`
- clave inicial: `Cambiar123*`

### Endpoints clave
- Swagger: `http://localhost:8080/swagger`
- Health: `http://localhost:8080/health`
- Hangfire: `http://localhost:8080/hangfire`
- Auth: `POST /api/seguridad/auth/login`
- Recupero clave: `POST /api/seguridad/auth/solicitar-recupero-clave`

## Frontend
1. `cd src/frontend/wellness-center`
2. `npm install`
3. `npm run start`
4. `npm run build`
5. `npm run test -- --watch=false --browsers=ChromeHeadless`

## Docker
Desde `infraestructura/docker`:
1. `cp .env.ejemplo .env`
2. completar secretos del archivo `.env`
3. `docker compose up --build`
4. API en `http://localhost:8080`
5. Frontend en `http://localhost:4200`

## Seguridad implementada
- JWT firmado con clave por entorno.
- Refresh token criptografico con RNG + hash SHA256 en persistencia.
- Rotacion de refresh token.
- Revocacion de sesiones por usuario.
- Recupero de clave preparado con token temporal.
- Bloqueo temporal por intentos fallidos.

## Profundidad funcional actual
- Clientes: listado, alta, detalle, edicion, baja logica, filtros y auditoria.
- Gastronomia: alta de pedido con detalle, cambio de estado, pagos, actualizacion cocina, consumo de stock y mermas por cancelacion.
- Gimnasio: CRUD inicial de socios, membresias, renovacion, QR firmado, check-in y historial de accesos.
- Integraciones: webhooks idempotentes + procesamiento por job.
