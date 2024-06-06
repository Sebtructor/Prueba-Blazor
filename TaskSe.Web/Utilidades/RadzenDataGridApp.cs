using Radzen.Blazor;

namespace TaskSe.Web.Utilidades
{
    public class RadzenDataGridApp<TItem> : RadzenDataGrid<TItem>
    {
        public RadzenDataGridApp() : base()
        {
            FilterCaseSensitivity = Radzen.FilterCaseSensitivity.CaseInsensitive;
            FilterMode = Radzen.FilterMode.SimpleWithMenu;
            ColumnWidth = "200px";
            //Traducción a español
            AndOperatorText = "Y";
            EqualsText = "Igual a";
            NotEqualsText = "No es igual a";
            LessThanText = "Menor qué";
            LessThanOrEqualsText = "Menor que o igual";
            GreaterThanText = "Mayor qué";
            GreaterThanOrEqualsText = "Mayor que o Igual";
            IsNullText = "Es nulo";
            IsNotNullText = "No es nulo";
            AndOperatorText = "Y";
            OrOperatorText = "O";
            ContainsText = "Contiene";
            DoesNotContainText = "No contiene";
            StartsWithText = "Inicia con";
            EndsWithText = "Termina con";
            ClearFilterText = "Limpiar";
            ApplyFilterText = "Aplicar";
            FilterText = "Filtrar";
            IsEmptyText = "Está vacío";
            IsNotEmptyText = "No está vacío";
            PageSizeText = "Página";
            EmptyText = "No hay registros para mostrar";
            PageSizeText = "Registros por página";

            //Resumen de tabla
            string pagingSummaryFormat = "Mostrando página {0} de {1} ({2} registro(s) en total)";
            PagingSummaryFormat = pagingSummaryFormat;

            //Pagesize options
            IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 30 };
            PageSizeOptions = pageSizeOptions;

            ShowPagingSummary = true;
            Responsive = true;
        }
    }
}
