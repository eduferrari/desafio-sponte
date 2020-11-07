using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class Usuarios
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public byte Ativo { get; set; }
    }
}
