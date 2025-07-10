namespace Messager;

/// <summary>
/// Represents a void type, since void is not a valid return type in C#
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    private static readonly Unit _value = new();

    /// <summary>
    /// Default and only value of the Unit type
    /// </summary>
    public static ref readonly Unit Value => ref _value;

    /// <summary>
    /// Task from Unit type
    /// </summary>
    public static Task<Unit> Task => System.Threading.Tasks.Task.FromResult(_value);

    /// <summary>
    /// Compares the current object with another object of the same type
    /// </summary>
    /// <param name="other">An object to compare with this object</param>
    /// <returns>Always returns 0 as all Unit values are equal</returns>
    public int CompareTo(Unit other) => 0;

    /// <summary>
    /// Compares the current instance with another object of the same type
    /// </summary>
    /// <param name="obj">An object to compare with this instance</param>
    /// <returns>Always returns 0 as all Unit values are equal</returns>
    public int CompareTo(object? obj) => 0;

    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified Unit value
    /// </summary>
    /// <param name="other">A Unit value to compare to this instance</param>
    /// <returns>Always returns true as all Unit values are equal</returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified object
    /// </summary>
    /// <param name="obj">An object to compare with this instance</param>
    /// <returns>true if obj is a Unit value; otherwise, false</returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Returns the hash code for this instance
    /// </summary>
    /// <returns>Always returns 0 as all Unit values are equal</returns>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Returns the string representation of the Unit value
    /// </summary>
    /// <returns>Always returns "()"</returns>
    public override string ToString() => "()";

    /// <summary>
    /// Determines whether two Unit values are equal
    /// </summary>
    /// <param name="first">The first Unit value to compare</param>
    /// <param name="second">The second Unit value to compare</param>
    /// <returns>Always returns true as all Unit values are equal</returns>
    public static bool operator ==(Unit first, Unit second) => true;

    /// <summary>
    /// Determines whether two Unit values are not equal
    /// </summary>
    /// <param name="first">The first Unit value to compare</param>
    /// <param name="second">The second Unit value to compare</param>
    /// <returns>Always returns false as all Unit values are equal</returns>
    public static bool operator !=(Unit first, Unit second) => false;
}