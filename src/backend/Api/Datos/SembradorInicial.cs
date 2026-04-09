namespace Api.Datos;

public sealed class SembradorInicial
{
    private readonly ContextoWellness _contexto;

    public SembradorInicial(ContextoWellness contexto)
    {
        _contexto = contexto;
    }

    public async Task SemillarAsync()
    {
        if (_contexto.Empresas.Any())
        {
            return;
        }

        var empresa = new Empresa { NombreLegal = "Kaizen Wellness Center", CodigoInterno = "KWC" };
        var sede = new Sede { EmpresaId = empresa.Id, Nombre = "Sede Central", Direccion = "Av Principal 100", CreadoPor = "semilla" };
        var usuario = new UsuarioSistema { Email = "admin@kaizen.local", ClaveHash = "hash-temporal", CreadoPor = "semilla" };
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

        _contexto.Empresas.Add(empresa);
        _contexto.Sedes.Add(sede);
        _contexto.Usuarios.Add(usuario);
        _contexto.Clientes.Add(cliente);
        _contexto.RegistrosAuditoria.Add(new RegistroAuditoria
        {
            Modulo = "sistema",
            Accion = "semilla-inicial",
            Usuario = "semilla",
            DetalleJson = "{\"estado\":\"ok\"}"
        });

        await _contexto.SaveChangesAsync();
    }
}
