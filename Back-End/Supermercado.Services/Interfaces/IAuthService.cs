using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermercado.Services.DTOs;

namespace Supermercado.Services.Interfaces;

public interface IAuthService
{
    TokenResponseDTO? Autenticar(LoginDTO dto);
}