namespace Domain.Shared;

public class FlexContext
{
    private readonly HashSet<object> _context = [];
    public FlexContext(params object[] values)
    {
        foreach(var value in values)
        {
            TryAdd(value);
        }
    }

    public bool TryAdd(object value)
    {
        return _context.Add(value);
    }

    public T Get<T>()
    {
        var values = _context.Where(key => key is T);
        if(!values.Any())
            throw new ArgumentException($"No key of type: {typeof(T)} found in context.");
        if(values.Count()!=1)
            throw new InvalidOperationException("You should not register in the context objects of the same type");
        return (T)values.First();
    }
    public static Result<FlexContext> StartContext(params object[] values) => Result<FlexContext>.Success(new FlexContext(values));
    public static Task<Result<FlexContext>> StartContextAsync(params object[] values) => Task.FromResult(Result<FlexContext>.Success(new FlexContext(values)));
}