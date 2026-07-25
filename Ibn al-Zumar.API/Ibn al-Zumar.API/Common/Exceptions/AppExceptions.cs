namespace IbnAlZumar.API.Common.Exceptions
{
    /// <summary>Thrown when a requested entity does not exist. Maps to HTTP 404.</summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    /// <summary>Thrown for business-rule/validation failures (duplicate SKU, invalid FK, ...). Maps to HTTP 400.</summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}