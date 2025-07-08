using Ardalis.GuardClauses;

namespace TaskManager.Domain.ValueObjects;

public sealed class UserId : IEquatable<UserId>
{
  public Guid Value { get; }

  public UserId(Guid value)
  {
    Value = Guard.Against.Default(value, nameof(value));
  }

  public static UserId Create() => new(Guid.NewGuid());
  public static UserId From(Guid value) => new(value);

  public bool Equals(UserId? other)
  {
    return other is not null && Value.Equals(other.Value);
  }

  public override bool Equals(object? obj)
  {
    return obj is UserId other && Equals(other);
  }

  public override int GetHashCode()
  {
    return Value.GetHashCode();
  }

  public static bool operator ==(UserId left, UserId right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(UserId left, UserId right)
  {
    return !left.Equals(right);
  }

  public override string ToString() => Value.ToString();

  public static implicit operator Guid(UserId userId) => userId.Value;
  public static implicit operator UserId(Guid value) => new(value);
}
