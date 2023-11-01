namespace DartRad.Models
{
    public class AuthResult
    {
        public AppUser User { get; set; }
        public string Error { get; set; }

        public bool IsSuccess => Error == null;
        public static AuthResult Success(AppUser user)
        {
            return new AuthResult { User = user };
        }

        public static AuthResult Failure(string errorMessage)
        {
            return new AuthResult { Error = errorMessage };
        }
    }
}
