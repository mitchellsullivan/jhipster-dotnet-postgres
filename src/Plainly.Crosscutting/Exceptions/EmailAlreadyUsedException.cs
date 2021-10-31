using Plainly.Crosscutting.Constants;

namespace Plainly.Crosscutting.Exceptions
{
    public class EmailAlreadyUsedException : BadRequestAlertException
    {
        public EmailAlreadyUsedException() : base(ErrorConstants.EmailAlreadyUsedType, "Email is already in use!",
            "userManagement", "emailexists")
        {
        }
    }
}
