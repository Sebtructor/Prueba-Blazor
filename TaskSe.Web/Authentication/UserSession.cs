namespace TaskSe.Web.Authentication
{
    public class UserSession
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
        public string Ip { get; set; } = string.Empty;
        public DateTime? last_login { get; set; }
        public string hash_password { get; set; } = string.Empty;
        public string id_auditoria_login { get; set; } = string.Empty;
        public bool enabled_2fa { get; set; } = false;
    }
}
