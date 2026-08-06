using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Services.DTOs;

public record LoginDTO(string Usuario, string Senha);

public record TokenResponseDTO(string Token, DateTime Expiracao);