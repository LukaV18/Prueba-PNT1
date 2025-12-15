using System.Threading.Tasks;
using Agenda_C.Data;
using Agenda_C.Models;
using Microsoft.EntityFrameworkCore;

namespace Agenda_C.Helpers
{
    public static class ValidationHelper
    {
        // ✅ Cambiado idExcluir a string
        public static async Task<bool> DniExisteAsync(AgendaContext context, int dni, string idExcluir = null)
        {
            var existeEnPacientes = await context.Pacientes
                .AnyAsync(p => p.DNI == dni && (idExcluir == null || p.Id != idExcluir));

            if (existeEnPacientes)
                return true;

            var existeEnProfesionales = await context.Profesionales
                .AnyAsync(p => p.DNI == dni && (idExcluir == null || p.Id != idExcluir));

            return existeEnProfesionales;
        }

        // ✅ Cambiado idExcluir a string
        public static async Task<bool> EmailExisteAsync(AgendaContext context, string email, string idExcluir = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var existeEnPacientes = await context.Pacientes
                .AnyAsync(p => p.Email == email && (idExcluir == null || p.Id != idExcluir));

            if (existeEnPacientes)
                return true;

            var existeEnProfesionales = await context.Profesionales
                .AnyAsync(p => p.Email == email && (idExcluir == null || p.Id != idExcluir));

            return existeEnProfesionales;
        }

        // ✅ Cambiado idExcluir a string
        public static async Task<bool> UserNameExisteAsync(AgendaContext context, string userName, string idExcluir = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            var existeEnPacientes = await context.Pacientes
                .AnyAsync(p => p.UserName == userName && (idExcluir == null || p.Id != idExcluir));

            if (existeEnPacientes)
                return true;

            var existeEnProfesionales = await context.Profesionales
                .AnyAsync(p => p.UserName == userName && (idExcluir == null || p.Id != idExcluir));

            return existeEnProfesionales;
        }

        // 🔹 El resto queda igual porque usan entidades con PK int
        public static async Task<bool> NumeroCredencialExisteAsync(AgendaContext context, int numeroCredencial, int? idExcluir = null)
        {
            return await context.Coberturas
                .AnyAsync(c => c.NumeroCredencial == numeroCredencial && (idExcluir == null || c.Id != idExcluir));
        }

        public static async Task<bool> MatriculaExisteAsync(AgendaContext context, int numero, TipoMatricula tipo, Provincia provincia, int? idExcluir = null)
        {
            return await context.Matriculas
                .AnyAsync(m => m.Numero == numero
                            && m.Tipo == tipo
                            && m.Provincia == provincia
                            && (idExcluir == null || m.Id != idExcluir));
        }

        public static async Task<bool> MatriculaTieneProfesionalAsync(AgendaContext context, int matriculaId)
        {
            return await context.Matriculas
                .AnyAsync(m => m.Id == matriculaId && m.ProfesionalId.HasValue);
        }

        public static async Task<bool> DireccionTienePersonaAsync(AgendaContext context, int direccionId)
        {
            return await context.Direcciones
                .AnyAsync(d => d.Id == direccionId && d.PersonaId != null);
        }

        public static async Task<bool> ProfesionalTieneTurnosAsync(AgendaContext context, string profesionalId)
        {
            return await context.Turnos
                .AnyAsync(t => t.ProfesionalId == profesionalId);
        }

        public static async Task<bool> CoberturaTienePacientesAsync(AgendaContext context, int coberturaId)
        {
            return await context.Pacientes
                .AnyAsync(p => p.CoberturaId == coberturaId);
        }
    }
}
