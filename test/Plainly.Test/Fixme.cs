using Plainy.Infrastructure.Data;
using Plainy.Domain;
using Plainy.Test.Setup;

namespace Plainy.Test
{
    public static class Fixme
    {
        public static User ReloadUser<TEntryPoint>(AppWebApplicationFactory<TEntryPoint> factory, User user)
            where TEntryPoint : class
        {
            var applicationDatabaseContext = factory.GetRequiredService<ApplicationDatabaseContext>();
            applicationDatabaseContext.Entry(user).Reload();
            return user;
        }
    }
}
