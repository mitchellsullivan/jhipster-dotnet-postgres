using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Plainly.Domain
{
    public class Role : IdentityRole<string>
    {
        public ICollection<User> Users { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
