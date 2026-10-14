namespace Domain.Abstractions;

/// <summary>
/// Represents a single validation error for a property: the error code (e.g. "User.PseudoLengthInvalid")
/// and any positional args used to render the localized message.
/// </summary>
public sealed record ValidationFieldError(string Code, IReadOnlyList<object>? Args)
{
   // See ResultError.Equals — same reasoning: compare Args by content, not by reference.
   public bool Equals(ValidationFieldError? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;
      if (Code != other.Code)
         return false;
      if (Args is null && other.Args is null)
         return true;
      if (Args is null || other.Args is null)
         return false;
      if (Args.Count != other.Args.Count)
         return false;
      for (int i = 0; i < Args.Count; i++)
      {
         if (!Equals(Args[i], other.Args[i]))
            return false;
      }

      return true;
   }

   public override int GetHashCode() => Code.GetHashCode(StringComparison.Ordinal);
}
