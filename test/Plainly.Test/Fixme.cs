using Plainly.Infrastructure.Data;
using Plainly.Domain;
using Plainly.Test.Setup;

namespace Plainly.Test
{
    public static class Fixme
    {
        public static User ReloadUser<TEntryPoint>(AppWebApplicationFactory<TEntryPoint> factory, User user)
            where TEntryPoint : class
        {
            var applicationDatabaseContext = factory.GetRequiredService<AppDbContext>();
            applicationDatabaseContext.Entry(user).Reload();
            return user;
        }
    }
}
