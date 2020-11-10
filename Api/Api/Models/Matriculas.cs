using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Api.Models
{
    public partial class Matriculas
    {
        public int MatriculaId { get; set; }
        public int AlunoId { get; set; }
        public string Cursos { get; set; }
        public double ValorTotal { get; set; }
        public string Data { get; set; }
    }
}
