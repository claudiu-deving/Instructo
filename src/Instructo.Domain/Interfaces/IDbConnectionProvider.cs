namespace Instructo.Domain.Interfaces;

public interface IDbConnectionProvider
{
    string ConnectionString { get; }
}
