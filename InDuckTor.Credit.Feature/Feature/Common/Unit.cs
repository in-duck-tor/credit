namespace InDuckTor.Credit.Feature.Feature.Common;

[Serializable]
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Default = new();

    public override int GetHashCode() => 0;

    public override bool Equals(object? obj) => obj is Unit;

    public override string ToString() => "()";

    public bool Equals(Unit other) => true;

    public static bool operator ==(Unit lhs, Unit rhs) => true;

    public static bool operator !=(Unit lhs, Unit rhs) => false;

    public static bool operator >(Unit lhs, Unit rhs) => false;

    public static bool operator >=(Unit lhs, Unit rhs) => true;

    public static bool operator <(Unit lhs, Unit rhs) => false;

    public static bool operator <=(Unit lhs, Unit rhs) => true;

    public int CompareTo(Unit other) => 0;

    public static Unit operator +(Unit a, Unit b) => Default;

    public static implicit operator ValueTuple(Unit _) => default;

    public static implicit operator Unit(ValueTuple _) => default;
}