using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Paciente : Usuario
    {
        // Relación con Cobertura
        public int? CoberturaId { get; set; }   // 🔹 FK como int?, porque Cobertura.Id es int
        public Cobertura Cobertura { get; set; }

        // Relación con Turnos
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();

        // Relación con Formularios
        public ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
        
    }
}