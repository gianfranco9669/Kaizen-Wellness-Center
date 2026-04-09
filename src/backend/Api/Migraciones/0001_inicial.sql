create schema if not exists plataforma;

create table if not exists plataforma.empresas (
    id uuid primary key,
    nombre_legal varchar(160) not null,
    codigo_interno varchar(32) not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);

create table if not exists plataforma.sedes (
    id uuid primary key,
    empresa_id uuid not null references plataforma.empresas(id),
    nombre varchar(120) not null,
    direccion varchar(240) not null,
    zona_horaria varchar(80) not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);

create table if not exists plataforma.usuarios (
    id uuid primary key,
    empresa_id uuid not null references plataforma.empresas(id),
    nombre_completo varchar(140) not null,
    email varchar(120) not null unique,
    clave_hash text not null,
    clave_salt text not null,
    bloqueado_temporalmente boolean not null,
    bloqueado_hasta_utc timestamp null,
    intentos_fallidos int not null,
    requiere_cambio_clave boolean not null,
    activo boolean not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);

create table if not exists plataforma.roles (
    id uuid primary key,
    empresa_id uuid not null references plataforma.empresas(id),
    nombre varchar(80) not null,
    codigo varchar(80) not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null,
    unique(empresa_id, codigo)
);

create table if not exists plataforma.permisos (
    id uuid primary key,
    codigo varchar(80) not null unique,
    descripcion varchar(250) not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);

create table if not exists plataforma.usuarios_roles (
    id uuid primary key,
    usuario_sistema_id uuid not null references plataforma.usuarios(id),
    rol_sistema_id uuid not null references plataforma.roles(id),
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null,
    unique(usuario_sistema_id, rol_sistema_id)
);

create table if not exists plataforma.roles_permisos (
    id uuid primary key,
    rol_sistema_id uuid not null references plataforma.roles(id),
    permiso_sistema_id uuid not null references plataforma.permisos(id),
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null,
    unique(rol_sistema_id, permiso_sistema_id)
);

create table if not exists plataforma.refresh_tokens_sesiones (
    id uuid primary key,
    usuario_sistema_id uuid not null references plataforma.usuarios(id),
    token_hash varchar(128) not null unique,
    expira_utc timestamp not null,
    revocado_utc timestamp null,
    ip_creacion varchar(50) null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);

create table if not exists plataforma.clientes (
    id uuid primary key,
    empresa_id uuid not null references plataforma.empresas(id),
    codigo_cliente varchar(32) not null,
    nombres varchar(120) not null,
    apellidos varchar(120) not null,
    documento varchar(30) not null,
    telefono varchar(30) not null,
    email varchar(120) not null,
    es_vip boolean not null,
    saldo_cuenta numeric(18,2) not null default 0,
    estado varchar(32) not null,
    restricciones_alimentarias text not null,
    notas_internas text not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);

create table if not exists plataforma.socios_gimnasio (
    id uuid primary key,
    cliente_id uuid not null references plataforma.clientes(id),
    numero_socio varchar(40) not null unique,
    estado_acceso varchar(30) not null,
    fecha_vencimiento_membresia_utc timestamp not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);

create table if not exists plataforma.membresias (
    id uuid primary key,
    socio_gimnasio_id uuid not null references plataforma.socios_gimnasio(id),
    plan_nombre varchar(120) not null,
    precio numeric(18,2) not null,
    fecha_inicio_utc timestamp not null,
    fecha_fin_utc timestamp not null,
    estado varchar(30) not null,
    fecha_creacion_utc timestamp not null,
    fecha_actualizacion_utc timestamp null,
    eliminado_logico boolean not null,
    creado_por varchar(80) not null,
    actualizado_por varchar(80) null
);
