// File: Common/Exceptions/AppExceptions.cs
using Microsoft.AspNetCore.Http;

namespace IbnAlZumar.Api.Common.Exceptions;

/// <summary>
/// Base type for exceptions the middleware treats as "expected" application errors — logged as
/// warnings (not errors) and mapped to the status code the exception itself carries, instead of
/// always returning a generic 500.
/// </summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundAppException : AppException
{
    public NotFoundAppException(string message) : base(message, StatusCodes.Status404NotFound) { }
}

public class ValidationAppException : AppException
{
    public IDictionary<string, string[]>? Errors { get; }

    public ValidationAppException(string message, IDictionary<string, string[]>? errors = null)
        : base(message, StatusCodes.Status400BadRequest)
    {
        Errors = errors;
    }
}

public class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message = "Invalid credentials.")
        : base(message, StatusCodes.Status401Unauthorized) { }
}

public class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message = "You do not have permission to perform this action.")
        : base(message, StatusCodes.Status403Forbidden) { }
}

public class ConflictAppException : AppException
{
    public ConflictAppException(string message) : base(message, StatusCodes.Status409Conflict) { }
}