namespace Shared.ApplicationInfrastructure
{
    public class ApplicationContext
    {
        public int? UserId { get; init; }
        public string UserRole { get; init; }
        public string? Token { get; init; }
        public string? ClientIpAddress { get; init; }
        public int Tier { get; init; }
        public ApplicationContext(string? clientIpAddress)
        {
            ClientIpAddress = clientIpAddress;
        }

        public ApplicationContext(int userId, string role, string token, string? clientIpAddress)
        {
            UserId = userId;
            UserRole = role;
            Token = token;
            ClientIpAddress = clientIpAddress;
        }
    }
}
