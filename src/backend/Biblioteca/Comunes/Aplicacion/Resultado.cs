namespace Comunes.Aplicacion;

public sealed class Resultado<T>
{
    public bool Exitoso { get; }
    public string? Error { get; }
    public T? Valor { get; }

    private Resultado(bool exitoso, T? valor, string? error)
    {
        Exitoso = exitoso;
        Valor = valor;
        Error = error;
    }

    public static Resultado<T> Ok(T valor) => new(true, valor, null);
    public static Resultado<T> Fallo(string error) => new(false, default, error);
}
