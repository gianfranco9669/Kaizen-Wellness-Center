using Microsoft.EntityFrameworkCore;

namespace Api.Datos;

public sealed class ContextoWellness : DbContext
{
    public ContextoWellness(DbContextOptions<ContextoWellness> options) : base(options)
    {
    }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Sede> Sedes => Set<Sede>();
    public DbSet<UsuarioSistema> Usuarios => Set<UsuarioSistema>();
    public DbSet<RolSistema> Roles => Set<RolSistema>();
    public DbSet<PermisoSistema> Permisos => Set<PermisoSistema>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<SocioGimnasio> SociosGimnasio => Set<SocioGimnasio>();
    public DbSet<Membresia> Membresias => Set<Membresia>();
    public DbSet<ProductoVenta> ProductosVenta => Set<ProductoVenta>();
    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<EventoWebhookExterno> EventosWebhookExternos => Set<EventoWebhookExterno>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("plataforma");

        modelBuilder.Entity<Cliente>().ToTable("clientes");
        modelBuilder.Entity<MovimientoStock>().ToTable("movimientos_stock");
        modelBuilder.Entity<RegistroAuditoria>().ToTable("registros_auditoria");
        modelBuilder.Entity<EventoWebhookExterno>().ToTable("eventos_webhook_externos")
            .HasIndex(x => x.IdempotenciaKey).IsUnique();

        modelBuilder.Entity<SocioGimnasio>().ToTable("socios_gimnasio")
            .HasIndex(x => x.NumeroSocio).IsUnique();

        modelBuilder.Entity<Cliente>().HasIndex(x => new { x.EmpresaId, x.Documento });
        modelBuilder.Entity<Pedido>().ToTable("pedidos");
        modelBuilder.Entity<Membresia>().ToTable("membresias");
        modelBuilder.Entity<ProductoVenta>().ToTable("productos_venta");
    }
}
