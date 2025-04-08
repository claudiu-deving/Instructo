namespace Domain.Interfaces;

public interface IDbConnectionProvider
{
    string ConnectionString { get; }
}
