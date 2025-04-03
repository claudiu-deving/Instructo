namespace Instructo.Domain.ValueObjects;

public abstract class ValueObject : IComparable
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if(left is null^right is null)
            return false;

        return left?.Equals(right!)!=false;
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if(obj==null||obj.GetType()!=GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach(var component in GetEqualityComponents())
        {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }

    public int CompareTo(object? obj)
    {
        if(obj==null)
            return 1;
        var other = (ValueObject)obj;
        using var thisComponents = GetEqualityComponents().GetEnumerator();
        using var otherComponents = other.GetEqualityComponents().GetEnumerator();
        while(thisComponents.MoveNext()&&otherComponents.MoveNext())
        {
            var comparison = Comparer<object>.Default.Compare(thisComponents.Current, otherComponents.Current);
            if(comparison!=0)
                return comparison;
        }
        return 0;
    }

    public static bool operator ==(ValueObject one, ValueObject two)
    {
        return EqualOperator(one, two);
    }
    public static bool operator !=(ValueObject one, ValueObject two)
    {
        return NotEqualOperator(one, two);
    }
}
