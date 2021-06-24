using Plainy.Crosscutting.Constants;

namespace Plainy.Crosscutting.Exceptions
{
    public class InvalidPasswordException : BaseException
    {
        public InvalidPasswordException() : base(ErrorConstants.InvalidPasswordType, "Incorrect Password")
        {
            //Status = StatusCodes.Status400BadRequest
        }
    }
}
