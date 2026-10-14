using System.Globalization;

namespace Domain.Abstractions;

public enum ErrorType
{
   /// <summary>
   /// Les erreurs de validation se produisent lorsque les données d'entrée ne respectent pas les critères ou contraintes requis.
   /// Ces erreurs indiquent généralement que le client a fourni des données invalides, telles que des champs obligatoires manquants, des formats incorrects ou des valeurs hors des plages acceptables
   /// Equivalent à HTTP 400 Bad Request.
   /// </summary>
   Validation,

   /// <summary>
   /// Les erreurs d'authentification se produisent lorsque l'utilisateur n'est pas authentifié ou que les informations d'identification fournies sont invalides.
   /// Ces erreurs indiquent que l'utilisateur doit s'authentifier pour accéder à la ressource ou effectuer l'action demandée.
   /// Equivalent à HTTP 401 Unauthorized.
   /// </summary>
   Unauthorized,

   /// <summary>
   /// Les erreurs d'autorisation se produisent lorsque l'utilisateur est authentifié mais n'a pas les permissions nécessaires pour accéder à la ressource ou effectuer l'action demandée.
   /// Ces erreurs indiquent que l'utilisateur n'est pas autorisé à accéder à la ressource ou à effectuer l'action, même s'il est authentifié.
   /// Equivalent à HTTP 403 Forbidden.
   /// </summary>
   Forbidden,

   /// <summary>
   /// Les erreurs de non-trouvabilité se produisent lorsque la ressource demandée n'existe pas ou n'est pas accessible.
   /// Ces erreurs indiquent que la ressource demandée est introuvable, soit parce qu'elle n'existe pas, soit parce que l'utilisateur n'a pas les permissions nécessaires pour y accéder.
   /// Equivalent à HTTP 404 Not Found.
   /// </summary>
   NotFound,

   /// <summary>
   /// Les erreurs de conflit se produisent lorsque la requête ne peut être traitée en raison d'un conflit avec l'état actuel de la ressource.
   /// Ces erreurs indiquent que la ressource a été modifiée par un autre utilisateur entre le moment où elle a été récupérée et le moment où la requête a été envoyée.
   /// Equivalent à HTTP 409 Conflict.
   /// </summary>
   Conflict,

   /// <summary>
   /// Les erreurs de failure se produisent lorsque la requête échoue pour une raison inconnue ou non spécifiée.
   /// Ces erreurs indiquent que la requête a échoué, mais sans fournir de détails spécifiques sur la cause.
   /// Equivalent à HTTP 500 Internal Server Error.
   /// </summary>
   Failure,
}

public enum SuccessType
{
   Ok,
   Created,
   NoContent
}

#pragma warning disable CA1815 // Override Equals and GetHashCode are not necessary for this struct

public readonly struct Unit
{
   public static readonly Unit Value;
}

#pragma warning restore CA1815

public record ResultError(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
   /// <summary>
   /// Validation errors dictionary for validation failures.
   /// Key is the property name, value is the list of errors with code and args for each field.
   /// </summary>
   public IReadOnlyDictionary<string, ValidationFieldError[]>? ValidationErrors { get; init; }

   /// <summary>
   /// Optional positional arguments used to build the human-readable Message.
   /// Convention: primitives only (string, int, bool, decimal). No DateTime, enum, or complex objects.
   /// Front-end consumers re-render the localized message by substituting these args into the i18n template
   /// referenced by Code.
   /// </summary>
   public IReadOnlyList<object>? Args { get; init; }

   public ResultError(string Code, string Message, object[] messageArgs, ErrorType Type = ErrorType.Failure)
       : this(Code, string.Format(CultureInfo.InvariantCulture, Message, messageArgs), Type)
   {
      Args = messageArgs;
   }

   // The record's compiler-generated Equals compares collections by reference, which would
   // make ResultError instances created from the same helper twice (e.g. UserErrors.PseudoReserved("x")
   // called once in production code and once in a test) compare unequal. We override to preserve
   // the record's value-equality semantics over Args and ValidationErrors content.
   public virtual bool Equals(ResultError? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;
      return Code == other.Code
          && Message == other.Message
          && Type == other.Type
          && SequenceEqualNullable(Args, other.Args)
          && ValidationErrorsEqual(ValidationErrors, other.ValidationErrors);
   }

   public override int GetHashCode() => HashCode.Combine(Code, Message, Type);

   private static bool SequenceEqualNullable(IReadOnlyList<object>? a, IReadOnlyList<object>? b)
   {
      if (a is null && b is null)
         return true;
      if (a is null || b is null)
         return false;
      if (a.Count != b.Count)
         return false;
      for (int i = 0; i < a.Count; i++)
      {
         if (!Equals(a[i], b[i]))
            return false;
      }

      return true;
   }

   private static bool ValidationErrorsEqual(
      IReadOnlyDictionary<string, ValidationFieldError[]>? a,
      IReadOnlyDictionary<string, ValidationFieldError[]>? b)
   {
      if (a is null && b is null)
         return true;
      if (a is null || b is null)
         return false;
      if (a.Count != b.Count)
         return false;
      foreach (KeyValuePair<string, ValidationFieldError[]> kvp in a)
      {
         if (!b.TryGetValue(kvp.Key, out ValidationFieldError[]? bValue))
            return false;
         if (kvp.Value.Length != bValue.Length)
            return false;
         for (int i = 0; i < kvp.Value.Length; i++)
         {
            if (!Equals(kvp.Value[i], bValue[i]))
               return false;
         }
      }

      return true;
   }
}

public readonly record struct Result
{
   public static Result<Unit> Success(SuccessType type = SuccessType.Ok) => Result<Unit>.Success(Unit.Value, type);
   public static Result<T> Success<T>(T value, SuccessType type = SuccessType.Ok) => Result<T>.Success(value, type);

   public static Result<Unit> Failure(ResultError error) => Result<Unit>.Failure(error);
   public static Result<T> Failure<T>(ResultError error) => Result<T>.Failure(error);
}

public readonly record struct Result<T> : IResult
{
   public bool IsSuccess { get; }
   public bool IsFailure => !IsSuccess;
   private readonly T? _value;
   private readonly ResultError? _error;

   public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("No value for failure result");

   public ResultError Error => IsFailure
       ? _error!
       : throw new InvalidOperationException("No error for success result");
   public SuccessType SuccessType { get; }

   private Result(T value, SuccessType successType)
   {
      IsSuccess = true;
      _value = value;
      _error = null;
      SuccessType = successType;
   }

   private Result(ResultError error)
   {
      IsSuccess = false;
      _value = default;
      _error = error;
      SuccessType = default;
   }

   public static Result<T> Success(T value, SuccessType type = SuccessType.Ok) => new(value, type);
   public static Result<T> Failure(ResultError error) => new(error);

   public static implicit operator Result<T>(T value) => Success(value);
   public static implicit operator Result<T>(ResultError error) => Failure(error);

   public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<ResultError, TResult> onFailure)
      => IsSuccess ? onSuccess(Value) : onFailure(Error);

   public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
      => IsSuccess ? Result<TNew>.Success(mapper(Value), SuccessType) : Result<TNew>.Failure(Error);

   public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
      => IsSuccess ? Result<TNew>.Success(await mapper(Value), SuccessType) : Result<TNew>.Failure(Error);

   public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
      => IsSuccess ? binder(Value) : Result<TNew>.Failure(Error);

   public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> binder)
      => IsSuccess ? await binder(Value) : Result<TNew>.Failure(Error);

   public Result<TNew> Pipe<TNew>(Func<T, TNew> pipe)
      => IsSuccess
         ? Result<TNew>.Success(pipe(Value), SuccessType)
         : Result<TNew>.Failure(Error);

   public Result<T> Tap(Action<T> action)
   {
      if (IsSuccess)
         action(Value);

      return this;
   }
}
