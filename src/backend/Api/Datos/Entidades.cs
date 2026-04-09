using Comunes.Dominio;

namespace Api.Datos;

public sealed class Empresa : EntidadBase
{
    public string NombreLegal { get; set; } = string.Empty;
    public string CodigoInterno { get; set; } = string.Empty;
}

public sealed class Sede : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string ZonaHoraria { get; set; } = "America/Argentina/Buenos_Aires";
}

public sealed class UsuarioSistema : EntidadBase
{
    public string Email { get; set; } = string.Empty;
    public string ClaveHash { get; set; } = string.Empty;
    public bool BloqueadoTemporalmente { get; set; }
    public int IntentosFallidos { get; set; }
}

public sealed class RolSistema : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
}

public sealed class PermisoSistema : EntidadBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public sealed class Cliente : EntidadBase
{
    public Guid EmpresaId { get; set; }
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

public sealed class SocioGimnasio : EntidadBase
{
    public Guid ClienteId { get; set; }
    public string NumeroSocio { get; set; } = string.Empty;
    public string EstadoAcceso { get; set; } = "habilitado";
    public DateTime FechaVencimientoMembresiaUtc { get; set; }
}

public sealed class Membresia : EntidadBase
{
    public Guid SocioGimnasioId { get; set; }
    public string PlanNombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public DateTime FechaInicioUtc { get; set; }
    public DateTime FechaFinUtc { get; set; }
    public string Estado { get; set; } = "vigente";
}

public sealed class ProductoVenta : EntidadBase
{
    public Guid EmpresaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
    public decimal CostoTeorico { get; set; }
}

public sealed class MovimientoStock : EntidadBase
{
    public Guid SedeId { get; set; }
    public Guid ProductoVentaId { get; set; }
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
}

public sealed class EventoWebhookExterno : EntidadBase
{
    public string Proveedor { get; set; } = string.Empty;
    public string IdempotenciaKey { get; set; } = string.Empty;
    public string EstadoProcesamiento { get; set; } = "pendiente";
    public string CuerpoJson { get; set; } = string.Empty;
}

public sealed class RegistroAuditoria : EntidadBase
{
    public string Modulo { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string DetalleJson { get; set; } = string.Empty;
}
