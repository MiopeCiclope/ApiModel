using KivalitaAPI.Enum;

namespace KivalitaAPI.DTOs
{
    public class AuthDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public LoginTypeEnum Client { get; set; }
        public string GrantType { get; set; }
        public string RefreshToken { get; set; }
    }
}