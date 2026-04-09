using Microsoft.EntityFrameworkCore;

namespace Api.Datos;

public sealed class ContextoWellness : DbContext
{
    public ContextoWellness(DbContextOptions<ContextoWellness> options) : base(options) { }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Sede> Sedes => Set<Sede>();
    public DbSet<UsuarioSistema> Usuarios => Set<UsuarioSistema>();
    public DbSet<RolSistema> Roles => Set<RolSistema>();
    public DbSet<PermisoSistema> Permisos => Set<PermisoSistema>();
    public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();
    public DbSet<RolPermiso> RolesPermisos => Set<RolPermiso>();
    public DbSet<RefreshTokenSesion> RefreshTokensSesiones => Set<RefreshTokenSesion>();
    public DbSet<TokenRecuperoClave> TokensRecuperoClaves => Set<TokenRecuperoClave>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<SocioGimnasio> SociosGimnasio => Set<SocioGimnasio>();
    public DbSet<Membresia> Membresias => Set<Membresia>();
    public DbSet<HistorialAccesoGimnasio> HistorialAccesosGimnasio => Set<HistorialAccesoGimnasio>();
    public DbSet<CategoriaProducto> CategoriasProductos => Set<CategoriaProducto>();
    public DbSet<ProductoVenta> ProductosVenta => Set<ProductoVenta>();
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<Insumo> Insumos => Set<Insumo>();
    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<Merma> Mermas => Set<Merma>();
    public DbSet<EventoWebhookExterno> EventosWebhookExternos => Set<EventoWebhookExterno>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("plataforma");

        modelBuilder.Entity<UsuarioSistema>().ToTable("usuarios").HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<RolSistema>().ToTable("roles").HasIndex(x => new { x.EmpresaId, x.Codigo }).IsUnique();
        modelBuilder.Entity<PermisoSistema>().ToTable("permisos").HasIndex(x => x.Codigo).IsUnique();
        modelBuilder.Entity<UsuarioRol>().ToTable("usuarios_roles").HasIndex(x => new { x.UsuarioSistemaId, x.RolSistemaId }).IsUnique();
        modelBuilder.Entity<RolPermiso>().ToTable("roles_permisos").HasIndex(x => new { x.RolSistemaId, x.PermisoSistemaId }).IsUnique();
        modelBuilder.Entity<RefreshTokenSesion>().ToTable("refresh_tokens_sesiones").HasIndex(x => x.TokenHash).IsUnique();
        modelBuilder.Entity<TokenRecuperoClave>().ToTable("tokens_recupero_claves").HasIndex(x => x.TokenHash).IsUnique();

        modelBuilder.Entity<Cliente>().ToTable("clientes").HasIndex(x => new { x.EmpresaId, x.Documento });
        modelBuilder.Entity<SocioGimnasio>().ToTable("socios_gimnasio").HasIndex(x => x.NumeroSocio).IsUnique();
        modelBuilder.Entity<Membresia>().ToTable("membresias");
        modelBuilder.Entity<HistorialAccesoGimnasio>().ToTable("historial_accesos_gimnasio");

        modelBuilder.Entity<CategoriaProducto>().ToTable("categorias_productos");
        modelBuilder.Entity<ProductoVenta>().ToTable("productos_venta");
        modelBuilder.Entity<Receta>().ToTable("recetas");
        modelBuilder.Entity<Insumo>().ToTable("insumos");
        modelBuilder.Entity<MovimientoStock>().ToTable("movimientos_stock");

        modelBuilder.Entity<Pedido>().ToTable("pedidos");
        modelBuilder.Entity<DetallePedido>().ToTable("detalle_pedidos");
        modelBuilder.Entity<Pago>().ToTable("pagos");

        modelBuilder.Entity<Proveedor>().ToTable("proveedores");
        modelBuilder.Entity<Compra>().ToTable("compras");
        modelBuilder.Entity<Merma>().ToTable("mermas");

        modelBuilder.Entity<EventoWebhookExterno>().ToTable("eventos_webhook_externos")
            .HasIndex(x => x.IdempotenciaKey).IsUnique();
        modelBuilder.Entity<RegistroAuditoria>().ToTable("registros_auditoria");

        modelBuilder.Entity<DetallePedido>()
            .HasOne(x => x.Pedido)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.PedidoId);
    }
}
