namespace Plainly.Infrastructure.Configuration
{
    public class SecuritySettings
    {
        public Authentication Authentication { get; set; }
        public Cors Cors { get; set; }
        public bool EnforceHttps { get; set; }
    }
}
