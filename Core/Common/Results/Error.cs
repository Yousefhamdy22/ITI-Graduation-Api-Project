using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Results
{
    public readonly struct Error : IEquatable<Error>
    {
        public string Code { get; }
        public string Description { get; }
        public ErrorKind Type { get; }

        public Error(string code, string description, ErrorKind type)
        {
            Code = code;
            Description = description;
            Type = type;
        }


        public static Error Failure(string code = nameof(Failure), string description = "General failure.")
            => new Error(code, description, ErrorKind.Failure);

        public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected error.")
            => new Error(code, description, ErrorKind.Unexpected);

        public static Error Validation(string code = nameof(Validation), string description = "Validation error")
            => new Error(code, description, ErrorKind.Validation);

        public static Error Conflict(string code = nameof(Conflict), string description = "Conflict error")
            => new Error(code, description, ErrorKind.Conflict);

        public static Error NotFound(string code = nameof(NotFound), string description = "Not found error")
            => new Error(code, description, ErrorKind.NotFound);

        public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Unauthorized error")
            => new Error(code, description, ErrorKind.Unauthorized);

        public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden error")
            => new Error(code, description, ErrorKind.Forbidden);
        public static Error ServiceUnavailableException(string serviceName, Exception innerException)
           => new Error("ServiceUnavailable", $"{serviceName} is currently unavailable.", ErrorKind.Failure);

        public static Error Create(int type, string code, string description)
            => new Error(code, description, (ErrorKind)type);


        public override bool Equals(object obj)
            => obj is Error other && Equals(other);

        public bool Equals(Error other)
            => Code == other.Code &&
               Description == other.Description &&
               Type == other.Type;

        public override int GetHashCode()
            => HashCode.Combine(Code, Description, Type);

        public static bool operator ==(Error left, Error right)
            => left.Equals(right);

        public static bool operator !=(Error left, Error right)
            => !(left == right);


        public void Deconstruct(out string code, out string description, out ErrorKind type)
        {
            code = Code;
            description = Description;
            type = Type;
        }


        public override string ToString()
            => $"{Type}: {Code} - {Description}";
    }

}
