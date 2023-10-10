using System.Numerics;

namespace Uchu.NavMesh.Grid;

public class Edge : IEquatable<Edge>
{
    /// <summary>
    /// Start of the edge.
    /// </summary>
    public readonly Vector3 Start;

    /// <summary>
    /// End of the edge.
    /// </summary>
    public readonly Vector3 End;

    /// <summary>
    /// Creates the edge.
    /// </summary>
    /// <param name="start">Start of the edge.</param>
    /// <param name="end">End of the edge.</param>
    public Edge(Vector3 start, Vector3 end)
    {
        this.Start = start;
        this.End = end;
    }

    /// <summary>
    /// Returns if another edge is equal.
    /// </summary>
    /// <param name="other">The other edge to compare.</param>
    /// <returns>If the edges are equal.</returns>
    public bool Equals(Edge? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (this.Start.Equals(other.Start) && End.Equals(other.End)) || (this.Start.Equals(other.End) && End.Equals(other.Start));
    }
    
    /// <summary>
    /// Returns if another object is equal.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>If the objects are equal.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Edge) obj);
    }

    /// <summary>
    /// Returns the hash code of the object.
    /// </summary>
    /// <returns>The hash code of the object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Start, this.End) + HashCode.Combine(this.End, this.Start);
    }
}