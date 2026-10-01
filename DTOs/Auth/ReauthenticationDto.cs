namespace SFM_BE.DTOs.Auth
{
    public class ReauthenticationDto
    {
        public string? Password { get; set; }
        public string? IdToken { get; set; }
    }
}
