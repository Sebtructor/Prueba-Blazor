using Radzen.Blazor;

namespace TaskSe.Web.Utilidades
{
    public class ProgramadorEventosView<TItem> : RadzenScheduler<TItem>
    {
        public ProgramadorEventosView() : base()
        {
            TodayText = "Hoy";
        }
    }
}
