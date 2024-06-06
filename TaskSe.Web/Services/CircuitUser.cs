using TaskSe.Web.Authentication;

namespace TaskSe.Web.Services
{
    public class CircuitUser
    {
        public UserSession usuario { get; set; }
        public string CircuitId { get; set; }
    }
}
