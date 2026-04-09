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
