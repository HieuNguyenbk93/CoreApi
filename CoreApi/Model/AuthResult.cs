using Microsoft.AspNetCore.Identity;

namespace CoreApi.Model
{
    public class AuthResult<T>
    {
        public bool Succeeded { get; }
        public T Data { get; }
        public bool IsModelValid { get; }
        public IEnumerable<IdentityError> Errors { get; }

        private AuthResult(T data)
        {
            Succeeded = true;
            IsModelValid = true;
            Data = data;
        }

        private AuthResult(bool isSucceeded, bool isModelValid)
        {
            Succeeded = isSucceeded;
            IsModelValid = isModelValid;
        }

        private AuthResult(bool isSucceeded)
        {
            Succeeded = isSucceeded;
            IsModelValid = isSucceeded;
        }

        private AuthResult(bool isSucceeded, bool isModelValid, IEnumerable<IdentityError> errors)
        {
            Succeeded = isSucceeded;
            IsModelValid = isModelValid;
            Errors = errors;
        }

        public static AuthResult<T> UnvalidatedResult => new AuthResult<T>(false);
        public static AuthResult<T> UnauthorizedResult => new AuthResult<T>(false, true);
        public static AuthResult<T> UnsucceededResult(IEnumerable<IdentityError> errors)
        {
            return new AuthResult<T>(false, true, errors);
        }
        public static AuthResult<T> SucceededResult => new AuthResult<T>(true);
        public static AuthResult<T> TokenResult(T token)
        {
            return new AuthResult<T>(token);
        }
    }
}
