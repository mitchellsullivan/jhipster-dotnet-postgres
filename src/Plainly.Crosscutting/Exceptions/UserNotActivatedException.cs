using System.Security.Authentication;

namespace Plainy.Crosscutting.Exceptions
{
    public class UserNotActivatedException : AuthenticationException
    {
        public UserNotActivatedException(string message) : base(message)
        {
        }
    }
}
