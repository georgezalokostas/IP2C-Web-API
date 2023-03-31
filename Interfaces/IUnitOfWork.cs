namespace IP2C_Web_API.Interface;

public interface IUnitOfWork : IDisposable
{
    IIPDetails IPDetails { get; }
    IReport Report { get; }
    Task SaveAsync();
}

