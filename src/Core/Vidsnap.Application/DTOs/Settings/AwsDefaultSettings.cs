namespace Vidsnap.Application.DTOs.Settings
{
    public class AwsDefaultSettings
    {
        public string Region { get; set; } = null!;
        public Credentials Credentials { get; set; } = null!;

         
    }

    public class Credentials
    {
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string SessionToken { get; set; } = null!;
    }
}