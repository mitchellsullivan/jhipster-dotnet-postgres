namespace Plainly.Infrastructure.Configuration
{
    public class Jwt
    {
        public string Secret { get; set; }
        public string Base64Secret { get; set; }
        public int TokenValidityInSeconds { get; set; }
        public int TokenValidityInSecondsForRememberMe { get; set; }
    }
}