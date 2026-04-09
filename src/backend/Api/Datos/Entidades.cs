using Comunes.Dominio;

namespace Api.Datos;

public sealed class Empresa : EntidadBase
{
    public string NombreLegal { get; set; } = string.Empty;
    public string CodigoInterno { get; set; } = string.Empty;
    public ICollection<Sede> Sedes { get; set; } = new List<Sede>();
}

public sealed class Sede : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string ZonaHoraria { get; set; } = "America/Argentina/Buenos_Aires";
}

public sealed class UsuarioSistema : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ClaveHash { get; set; } = string.Empty;
    public string ClaveSalt { get; set; } = string.Empty;
    public bool BloqueadoTemporalmente { get; set; }
    public DateTime? BloqueadoHastaUtc { get; set; }
    public int IntentosFallidos { get; set; }
    public bool RequiereCambioClave { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();
}

public sealed class RolSistema : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public ICollection<RolPermiso> Permisos { get; set; } = new List<RolPermiso>();
}

public sealed class PermisoSistema : EntidadBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public sealed class UsuarioRol : EntidadBase
{
    public Guid UsuarioSistemaId { get; set; }
    public UsuarioSistema? UsuarioSistema { get; set; }
    public Guid RolSistemaId { get; set; }
    public RolSistema? RolSistema { get; set; }
}

public sealed class RolPermiso : EntidadBase
{
    public Guid RolSistemaId { get; set; }
    public RolSistema? RolSistema { get; set; }
    public Guid PermisoSistemaId { get; set; }
    public PermisoSistema? PermisoSistema { get; set; }
}

public sealed class RefreshTokenSesion : EntidadBase
{
    public Guid UsuarioSistemaId { get; set; }
    public UsuarioSistema? UsuarioSistema { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiraUtc { get; set; }
    public DateTime? RevocadoUtc { get; set; }
    public string? IpCreacion { get; set; }
}

public sealed class Cliente : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string CodigoCliente { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EsVip { get; set; }
    public decimal SaldoCuenta { get; set; }
    public string Estado { get; set; } = "activo";
    public string RestriccionesAlimentarias { get; set; } = string.Empty;
    public string NotasInternas { get; set; } = string.Empty;
}

public sealed class CategoriaProducto : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public sealed class ProductoVenta : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public Guid CategoriaProductoId { get; set; }
    public CategoriaProducto? CategoriaProducto { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
    public decimal CostoTeorico { get; set; }
    public bool Activo { get; set; } = true;
}

public sealed class Receta : EntidadBase
{
    public Guid ProductoVentaId { get; set; }
    public ProductoVenta? ProductoVenta { get; set; }
    public string Version { get; set; } = "1.0";
    public decimal Rendimiento { get; set; }
}

public sealed class Insumo : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal CostoUnitario { get; set; }
}

public sealed class MovimientoStock : EntidadBase
{
    public Guid SedeId { get; set; }
    public Guid? ProductoVentaId { get; set; }
    public Guid? InsumoId { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public sealed class Pedido : EntidadBase
{
    public Guid SedeId { get; set; }
    public Guid ClienteId { get; set; }
    public string Canal { get; set; } = "mostrador";
    public string Estado { get; set; } = "pendiente";
    public decimal Total { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}

public sealed class DetallePedido : EntidadBase
{
    public Guid PedidoId { get; set; }
    public Pedido? Pedido { get; set; }
    public Guid ProductoVentaId { get; set; }
    public ProductoVenta? ProductoVenta { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public sealed class Pago : EntidadBase
{
    public Guid PedidoId { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public decimal Importe { get; set; }
    public string Estado { get; set; } = "aprobado";
}

public sealed class SocioGimnasio : EntidadBase
{
    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public string NumeroSocio { get; set; } = string.Empty;
    public string EstadoAcceso { get; set; } = "habilitado";
    public DateTime FechaVencimientoMembresiaUtc { get; set; }
}

public sealed class Membresia : EntidadBase
{
    public Guid SocioGimnasioId { get; set; }
    public SocioGimnasio? SocioGimnasio { get; set; }
    public string PlanNombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public DateTime FechaInicioUtc { get; set; }
    public DateTime FechaFinUtc { get; set; }
    public string Estado { get; set; } = "vigente";
}

public sealed class HistorialAccesoGimnasio : EntidadBase
{
    public Guid SocioGimnasioId { get; set; }
    public DateTime FechaAccesoUtc { get; set; }
    public string TipoAcceso { get; set; } = "check-in";
    public string Resultado { get; set; } = "permitido";
}

public sealed class Proveedor : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public string NombreComercial { get; set; } = string.Empty;
    public string Cuit { get; set; } = string.Empty;
}

public sealed class Compra : EntidadBase
{
    public Guid ProveedorId { get; set; }
    public Guid SedeId { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "recibida";
}

public sealed class Merma : EntidadBase
{
    public Guid SedeId { get; set; }
    public Guid? ProductoVentaId { get; set; }
    public Guid? InsumoId { get; set; }
    public decimal Cantidad { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public sealed class EventoWebhookExterno : EntidadBase
{
    public string Proveedor { get; set; } = string.Empty;
    public string IdempotenciaKey { get; set; } = string.Empty;
    public string TipoEvento { get; set; } = string.Empty;
    public string EstadoProcesamiento { get; set; } = "pendiente";
    public string CuerpoJson { get; set; } = string.Empty;
    public int Reintentos { get; set; }
}

public sealed class RegistroAuditoria : EntidadBase
{
    public string Modulo { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public string DetalleJson { get; set; } = string.Empty;
}
