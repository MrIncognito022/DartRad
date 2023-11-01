using DartRad.Data;
using DartRad.Utilities;

namespace DartRad.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;

        public AuthService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<AuthResult> Authenticate(string email, string password, string role)
        {
            // verify that the role exists
            if (string.IsNullOrEmpty(role) || !AppRoles.ToList().Contains(role))
            {
                return AuthResult.Failure("Invalid Role provided");
            }

            BaseUser userFromDb;
            if (role == AppRoles.SuperAdmin)
            {
                userFromDb = _dbContext.SuperAdmin.FirstOrDefault(x => x.Email == email);
            }
            else if (role == AppRoles.Editor)
            {
                userFromDb = _dbContext.Editor.FirstOrDefault(x => x.Email == email);
            }
            else
            {
                userFromDb = _dbContext.ContentCreator.FirstOrDefault(x => x.Email == email);
            }

            if (userFromDb == null)
            {
                return AuthResult.Failure($"User with email: '{email}' was not found");
            }

            bool isPasswordVerified = AuthHelper.VerifyPassword(password, userFromDb.Password);

            if (!isPasswordVerified)
            {
                return AuthResult.Failure($"Invalid Password");
            }

            var user = new AppUser(userFromDb.Id, userFromDb.Name, email, role);
            return AuthResult.Success(user);
        }
    }
}
