
#nullable enable

namespace Agenda_C.Models
{
    public class ErrorViewModel
    {
        // Puede ser null, por eso usamos string?
        public string? RequestId { get; set; }

        // Devuelve true si RequestId no es null ni vacío
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

