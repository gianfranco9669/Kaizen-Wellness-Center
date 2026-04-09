using Api.Seguridad;

namespace Api.Datos;

public sealed class SembradorInicial
{
    private readonly ContextoWellness _contexto;
    private readonly IServicioHashClave _hash;

    public SembradorInicial(ContextoWellness contexto, IServicioHashClave hash)
    {
        _contexto = contexto;
        _hash = hash;
    }

    public async Task SemillarAsync()
    {
        if (_contexto.Empresas.Any()) return;

        var empresa = new Empresa { NombreLegal = "Kaizen Wellness Center SA", CodigoInterno = "KWC" };
        var sede = new Sede { EmpresaId = empresa.Id, Nombre = "Sede Central", Direccion = "Av Principal 100", CreadoPor = "semilla" };

        var permisoClientesEliminar = new PermisoSistema { Codigo = "clientes.eliminar", Descripcion = "Permite baja logica de clientes", CreadoPor = "semilla" };
        var permisoClientesEditar = new PermisoSistema { Codigo = "clientes.editar", Descripcion = "Permite edicion de clientes", CreadoPor = "semilla" };

        var rolAdmin = new RolSistema { EmpresaId = empresa.Id, Nombre = "Administrador", Codigo = "admin", CreadoPor = "semilla" };
        var rolRecepcion = new RolSistema { EmpresaId = empresa.Id, Nombre = "Recepcion", Codigo = "recepcion", CreadoPor = "semilla" };

        var (hash, salt) = _hash.GenerarHash("Cambiar123*");
        var usuarioAdmin = new UsuarioSistema
        {
            EmpresaId = empresa.Id,
            NombreCompleto = "Administrador Kaizen",
            Email = "admin@kaizen.local",
            ClaveHash = hash,
            ClaveSalt = salt,
            RequiereCambioClave = true,
            CreadoPor = "semilla"
        };

        var cliente = new Cliente
        {
            EmpresaId = empresa.Id,
            CodigoCliente = "CLI-000001",
            Nombres = "Valeria",
            Apellidos = "Mendez",
            Documento = "30123456",
            Telefono = "+5491112345678",
            Email = "valeria@cliente.com",
            EsVip = true,
            Estado = "activo",
            RestriccionesAlimentarias = "sin-gluten",
            NotasInternas = "cliente frecuente gastronomia",
            CreadoPor = "semilla"
        };

        var socio = new SocioGimnasio
        {
            ClienteId = cliente.Id,
            NumeroSocio = "SOC-00001",
            EstadoAcceso = "habilitado",
            FechaVencimientoMembresiaUtc = DateTime.UtcNow.AddMonths(1),
            CreadoPor = "semilla"
        };

        var membresia = new Membresia
        {
            SocioGimnasioId = socio.Id,
            PlanNombre = "Premium Integral",
            Precio = 50000,
            FechaInicioUtc = DateTime.UtcNow,
            FechaFinUtc = DateTime.UtcNow.AddMonths(1),
            Estado = "vigente",
            CreadoPor = "semilla"
        };

        var categoria = new CategoriaProducto { EmpresaId = empresa.Id, Nombre = "Sushi", CreadoPor = "semilla" };
        var producto = new ProductoVenta
        {
            EmpresaId = empresa.Id,
            CategoriaProductoId = categoria.Id,
            Codigo = "SUSHI-001",
            Nombre = "Roll Salmon Premium",
            PrecioVenta = 14500,
            CostoTeorico = 6200,
            CreadoPor = "semilla"
        };

        _contexto.Empresas.Add(empresa);
        _contexto.Sedes.Add(sede);
        _contexto.Permisos.AddRange(permisoClientesEliminar, permisoClientesEditar);
        _contexto.Roles.AddRange(rolAdmin, rolRecepcion);
        _contexto.Usuarios.Add(usuarioAdmin);

        _contexto.RolesPermisos.AddRange(
            new RolPermiso { RolSistemaId = rolAdmin.Id, PermisoSistemaId = permisoClientesEliminar.Id, CreadoPor = "semilla" },
            new RolPermiso { RolSistemaId = rolAdmin.Id, PermisoSistemaId = permisoClientesEditar.Id, CreadoPor = "semilla" },
            new RolPermiso { RolSistemaId = rolRecepcion.Id, PermisoSistemaId = permisoClientesEditar.Id, CreadoPor = "semilla" }
        );

        _contexto.UsuariosRoles.Add(new UsuarioRol { UsuarioSistemaId = usuarioAdmin.Id, RolSistemaId = rolAdmin.Id, CreadoPor = "semilla" });

        _contexto.Clientes.Add(cliente);
        _contexto.SociosGimnasio.Add(socio);
        _contexto.Membresias.Add(membresia);
        _contexto.CategoriasProductos.Add(categoria);
        _contexto.ProductosVenta.Add(producto);

        _contexto.RegistrosAuditoria.Add(new RegistroAuditoria
        {
            Modulo = "sistema",
            Accion = "semilla-inicial",
            Usuario = "semilla",
            Entidad = "empresa",
            DetalleJson = "{\"estado\":\"ok\"}"
        });

        await _contexto.SaveChangesAsync();
    }
}
