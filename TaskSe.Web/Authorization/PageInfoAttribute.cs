namespace TaskSe.Web.Authorization
{
    public class PageInfoAttribute : Attribute
    {
        public string id_modulo = "";
        public bool perfilable = true;

        public PageInfoAttribute(string id_modulo = "", bool perfilable = true)
        {
            this.id_modulo = id_modulo;
            this.perfilable = perfilable;
        }
    }
}
