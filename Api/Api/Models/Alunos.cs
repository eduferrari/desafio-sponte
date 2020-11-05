using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class Alunos
    {
        public int AlunoId { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public string DataNascimento { get; set; }
    }
}
