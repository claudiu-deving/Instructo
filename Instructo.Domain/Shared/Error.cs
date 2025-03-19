namespace Instructo.Domain.Shared;

public class Error(string code, string message) : IEquatable<Error>
{
    public string Code { get; } = code;
    public string Message { get; } = message;
    public static Error None => new Error(string.Empty, string.Empty);

    public bool Equals(Error? other)
    {
        if(other is null)
            return false;
        if(string.IsNullOrEmpty(Code)&&string.IsNullOrEmpty(other.Code))
            return true;
        if(string.IsNullOrEmpty(Code)||string.IsNullOrEmpty(other.Code))
            return false;

        return this.Code.Equals(other.Code);
    }

    public static implicit operator string(Error error) => error.Code;

    public static bool operator ==(Error? left, Error? right)
    {
        if(left is null&&right is null)
        {
            return true;
        }
        if(left is null||right is null)
        {
            return false;
        }
        return left.Code.Equals(right.Code);
    }

    public static bool operator !=(Error? left, Error? right)
    {
        if(left is null&&right is null)
        {
            return false;
        }
        if(left is null||right is null)
        {
            return true;
        }
        return !left.Code.Equals(right.Code);
    }
    public override bool Equals(object? obj)
    {
        if(obj is null)
            return false;
        return Equals(obj as Error);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode()*17;
    }
}