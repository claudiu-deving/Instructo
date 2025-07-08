namespace Domain.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task ExecuteInTransactionAsync(Func<Task> operation);
    Task SaveChangesAsync();
}